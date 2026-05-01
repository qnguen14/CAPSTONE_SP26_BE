using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Payment;
using AgroTemp.Domain.Entities;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using PayOS;
using PayOS.Models.V1.Payouts;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.V2.PaymentRequests.Invoices;
using PayOS.Models.Webhooks;
using System.Security.Claims;
using System.Text.Json;

namespace AgroTemp.Service.Implements;

public class PayOSService : IPayOSService
{
    private static readonly JsonSerializerOptions WebhookSerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly int[] LocalFrontendPorts = { 3000, 8081 };
    private const string DefaultLocalFrontendBaseUrl = "http://localhost:3000";
    private readonly PayOSClient _orderClient;
    private readonly PayOSClient _transferClient;
    private readonly IUnitOfWork<AgroTempDbContext> _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWalletService _walletService;
    private readonly IConfiguration _configuration;

    public PayOSService(
        [FromKeyedServices("OrderClient")] PayOSClient orderClient,
        [FromKeyedServices("TransferClient")] PayOSClient transferClient,
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IWalletService walletService,
        IConfiguration configuration)
    {
        _orderClient = orderClient;
        _transferClient = transferClient;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _walletService = walletService;
        _configuration = configuration;
    }

    public async Task<PayOSOrderResponse?> GetOrderAsync(Guid id)
    {
        var order = await GetOrderEntityByIdAsync(id);
        if (order == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(order.PaymentLinkId))
        {
            return MapOrderToResponse(order);
        }

        var paymentLink = await _orderClient.PaymentRequests.GetAsync(order.PaymentLinkId);
        UpdateOrderFromPaymentLink(order, paymentLink);
        await ReplaceTransactionsFromPaymentLinkAsync(order, paymentLink);
        await SyncDepositTransactionsToWalletAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return MapOrderToResponse(order);
    }

    public async Task<PayOSOrderResponse> CreatePaymentAsync(CreatePayOSOrderRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var farmer = await _unitOfWork.GetRepository<Farmer>()
            .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId.Value);

        if (farmer == null)
        {
            throw new Exception("Farmer profile not found.");
        }

        var primaryFarm = await _unitOfWork.GetRepository<Farm>()
            .FirstOrDefaultAsync(predicate: f => f.FarmerId == farmer.Id && f.IsPrimary);

        var buyerName = farmer.ContactName;
        var buyerCompanyName = primaryFarm?.LocationName;
        var buyerEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var returnUrl = BuildFrontendUrl("/farmer/payments/success");
        var cancelUrl = BuildFrontendUrl("/farmer/payments/cancel");
        var expiredAt = DateTimeOffset.UtcNow.AddHours(2);
        var buyerNotGetInvoice = false;
        int? taxPercentage = null;
        var normalizedDescription = NormalizeDescription(request.Description, $"AgroTemp DP {orderCode}");

        var hardcodedItem = new PaymentLinkItem
        {
            Name = "Nap tien vi",
            Quantity = 1,
            Price = request.TotalAmount,
            Unit = "goi",
            TaxPercentage = null
        };

        CreatePaymentLinkRequest BuildPaymentRequest(bool minimal)
        {
            var req = new CreatePaymentLinkRequest
            {
                OrderCode = orderCode,
                Amount = request.TotalAmount,
                Description = normalizedDescription,
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl,
                ExpiredAt = expiredAt.ToUnixTimeSeconds(),
                Items = new List<PaymentLinkItem> { hardcodedItem }
            };

            if (!minimal)
            {
                req.BuyerName = buyerName;
                req.BuyerEmail = buyerEmail;
                req.Invoice = new InvoiceRequest
                {
                    BuyerNotGetInvoice = buyerNotGetInvoice,
                    TaxPercentage = taxPercentage.HasValue ? (TaxPercentage?)taxPercentage.Value : null
                };
            }

            return req;
        }

        static bool IsGatewayUnavailable(Exception ex) =>
            ex.Message.Contains("Cổng thanh toán không tồn tại", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("tạm dừng", StringComparison.OrdinalIgnoreCase);

        CreatePaymentLinkResponse paymentResponse;
        try
        {
            // First try with richer payload for better invoice metadata.
            paymentResponse = await _orderClient.PaymentRequests.CreateAsync(BuildPaymentRequest(minimal: false));
        }
        catch (Exception ex) when (IsGatewayUnavailable(ex))
        {
            try
            {
                // Retry with minimal required payload using OrderClient.
                paymentResponse = await _orderClient.PaymentRequests.CreateAsync(BuildPaymentRequest(minimal: true));
            }
            catch (Exception exOrderMinimal) when (IsGatewayUnavailable(exOrderMinimal))
            {
                try
                {
                    // Final fallback: try TransferClient keys in case merchant only enabled one channel/key-set.
                    paymentResponse = await _transferClient.PaymentRequests.CreateAsync(BuildPaymentRequest(minimal: true));
                }
                catch (Exception exTransfer) when (IsGatewayUnavailable(exTransfer))
                {
                    throw new InvalidOperationException(
                        "PayOS gateway for this merchant is unavailable (not found or suspended). Please activate VietQR/payment gateway in PayOS Dashboard.",
                        exTransfer);
                }
            }
        }

        var order = new PayOSOrder
        {
            Id = Guid.NewGuid(),
            UserId = currentUserId,
            OrderCode = orderCode,
            TotalAmount = request.TotalAmount,
            Description = paymentResponse.Description,
            PaymentLinkId = paymentResponse.PaymentLinkId,
            QrCode = paymentResponse.QrCode,
            CheckoutUrl = paymentResponse.CheckoutUrl,
            Status = paymentResponse.Status.ToString(),
            Amount = paymentResponse.Amount,
            AmountPaid = 0,
            AmountRemaining = paymentResponse.Amount,
            Bin = paymentResponse.Bin,
            AccountNumber = paymentResponse.AccountNumber,
            AccountName = paymentResponse.AccountName,
            Currency = paymentResponse.Currency,
            ReturnUrl = returnUrl,
            CancelUrl = cancelUrl,
            CreatedAt = DateTime.UtcNow,
            BuyerName = buyerName,
            BuyerCompanyName = buyerCompanyName,
            BuyerEmail = buyerEmail,
            //BuyerPhone = buyerPhone,
            // BuyerAddress = buyerAddress,
            ExpiredAt = expiredAt.UtcDateTime,
            BuyerNotGetInvoice = buyerNotGetInvoice,
            TaxPercentage = taxPercentage,
            Items = new List<PayOSOrderItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Nap tien vi",
                    Quantity = 1,
                    Price = request.TotalAmount,
                    Unit = "goi",
                    TaxPercentage = null
                }
            }
        };

        await _unitOfWork.GetRepository<PayOSOrder>().InsertAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return MapOrderToResponse(order);
    }

    public async Task<PaymentLink?> CancelPaymentAsync(Guid orderId, string? cancellationReason)
    {
        var order = await GetOrderEntityByIdAsync(orderId);
        if (order == null || string.IsNullOrWhiteSpace(order.PaymentLinkId))
        {
            return null;
        }

        var paymentLink = await _orderClient.PaymentRequests.CancelAsync(order.PaymentLinkId, cancellationReason ?? "Cancelled by user");
        UpdateOrderFromPaymentLink(order, paymentLink);
        await ReplaceTransactionsFromPaymentLinkAsync(order, paymentLink);
        _unitOfWork.GetRepository<PayOSOrder>().UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return paymentLink;
    }

    public async Task<PayOSInvoicesInfoResponse?> GetInvoicesAsync(Guid orderId)
    {
        var order = await GetOrderEntityByIdAsync(orderId);
        if (order == null || string.IsNullOrWhiteSpace(order.PaymentLinkId))
        {
            return null;
        }

        var invoices = await _orderClient.PaymentRequests.Invoices.GetAsync(order.PaymentLinkId);

        var existingInvoices = await _unitOfWork.GetRepository<PayOSInvoice>()
            .GetListAsync(predicate: i => i.OrderId == orderId);
        if (existingInvoices.Any())
        {
            _unitOfWork.GetRepository<PayOSInvoice>().DeleteRangeAsync(existingInvoices);
        }

        var newInvoices = invoices.Invoices.Select(i => new PayOSInvoice
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            InvoiceId = i.InvoiceId,
            InvoiceNumber = i.InvoiceNumber,
            IssuedTimestamp = i.IssuedTimestamp,
            IssuedDatetime = i.IssuedDatetime,
            TransactionId = i.TransactionId,
            ReservationCode = i.ReservationCode,
            CodeOfTax = i.CodeOfTax
        }).ToList();

        if (newInvoices.Any())
        {
            await _unitOfWork.GetRepository<PayOSInvoice>().InsertRangeAsync(newInvoices);
        }

        await _unitOfWork.SaveChangesAsync();

        return new PayOSInvoicesInfoResponse
        {
            Invoices = newInvoices.Select(MapInvoiceToResponse).ToList()
        };
    }

    public async Task<(byte[] Content, string ContentType, string FileName)?> DownloadInvoiceAsync(Guid orderId, string invoiceId)
    {
        var order = await GetOrderEntityByIdAsync(orderId);
        if (order == null || string.IsNullOrWhiteSpace(order.PaymentLinkId))
        {
            return null;
        }

        var invoice = await _unitOfWork.GetRepository<PayOSInvoice>()
            .FirstOrDefaultAsync(predicate: i => i.OrderId == orderId && i.InvoiceId == invoiceId);
        if (invoice == null)
        {
            return null;
        }

        var invoiceFile = await _orderClient.PaymentRequests.Invoices.DownloadAsync(invoiceId, order.PaymentLinkId);
        await using var contentStream = invoiceFile.Content;
        using var memoryStream = new MemoryStream();
        await contentStream.CopyToAsync(memoryStream);

        return (
            memoryStream.ToArray(),
            invoiceFile.ContentType ?? "application/pdf",
            invoiceFile.FileName ?? $"invoice_{invoiceId}.pdf"
        );
    }

    public async Task<PayOSWebhookResultResponse> VerifyWebhookAsync(Webhook webhook)
    {
        var webhookLog = new PayOSWebhookLog
        {
            Id = Guid.NewGuid(),
            RawPayload = JsonSerializer.Serialize(webhook, WebhookSerializerOptions),
            IsPayloadParsed = webhook != null,
            ReceivedAt = DateTime.UtcNow
        };

        await _unitOfWork.GetRepository<PayOSWebhookLog>().InsertAsync(webhookLog);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            if (webhook == null)
            {
                throw new ArgumentException("Webhook payload is required.", nameof(webhook));
            }

            var webhookData = await _orderClient.Webhooks.VerifyAsync(webhook);
            webhookLog.IsVerified = true;
            webhookLog.OrderCode = webhookData.OrderCode;
            webhookLog.Reference = webhookData.Reference;

            if (webhookData.OrderCode == 123 && webhookData.Description == "VQRIO123" && webhookData.AccountNumber == "12345678")
            {
                webhookLog.ProcessedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<PayOSWebhookLog>().UpdateAsync(webhookLog);
                await _unitOfWork.SaveChangesAsync();

                return new PayOSWebhookResultResponse
                {
                    Message = "Webhook processed successfully",
                    OrderCode = webhookData.OrderCode
                };
            }

            var order = await _unitOfWork.Context.Set<PayOSOrder>()
            .Include(x => x.Transactions)
            .FirstOrDefaultAsync(x => x.OrderCode == webhookData.OrderCode);

            if (order != null)
            {
                webhookLog.OrderId = order.Id;

                var existingReference = order.Transactions.Any(x => x.Reference == webhookData.Reference);
                if (!existingReference)
                {
                    await _unitOfWork.GetRepository<PayOSTransaction>().InsertAsync(new PayOSTransaction
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        Reference = webhookData.Reference,
                        Amount = webhookData.Amount,
                        AccountNumber = webhookData.AccountNumber,
                        Description = webhookData.Description,
                        TransactionDateTime = ParseWebhookDateTime(webhookData.TransactionDateTime).UtcDateTime,
                        VirtualAccountName = webhookData.VirtualAccountName,
                        VirtualAccountNumber = webhookData.VirtualAccountNumber,
                        CounterAccountBankId = webhookData.CounterAccountBankId,
                        CounterAccountBankName = webhookData.CounterAccountBankName,
                        CounterAccountName = webhookData.CounterAccountName,
                        CounterAccountNumber = webhookData.CounterAccountNumber
                    });

                    await CreditWalletFromPayOSAsync(
                        order,
                        webhookData.Amount,
                        webhookData.Reference,
                        webhookData.Description);
                }

                var transactions = await _unitOfWork.GetRepository<PayOSTransaction>()
                    .GetListAsync(predicate: x => x.OrderId == order.Id);

                order.AmountPaid = transactions.Sum(x => x.Amount);
                order.AmountRemaining = order.Amount - order.AmountPaid;
                order.Status = order.AmountRemaining > 0 ? PaymentLinkStatus.Underpaid.ToString() : PaymentLinkStatus.Paid.ToString();
                order.LastTransactionUpdate = DateTime.UtcNow;

                _unitOfWork.GetRepository<PayOSOrder>().UpdateAsync(order);
            }

            webhookLog.ProcessedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<PayOSWebhookLog>().UpdateAsync(webhookLog);
            await _unitOfWork.SaveChangesAsync();

            return new PayOSWebhookResultResponse
            {
                Message = "Webhook processed successfully",
                OrderCode = webhookData.OrderCode
            };
        }
        catch (Exception ex)
        {
            webhookLog.ErrorMessage = ex.Message;
            webhookLog.ProcessedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<PayOSWebhookLog>().UpdateAsync(webhookLog);
            await _unitOfWork.SaveChangesAsync();
            throw;
        }
    }

    private static DateTimeOffset ParseWebhookDateTime(string dateTimeString)
    {
        if (DateTimeOffset.TryParse(dateTimeString, out var result))
        {
            return result;
        }

        if (DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var dateTime))
        {
            return new DateTimeOffset(dateTime, TimeSpan.FromHours(7));
        }

        return DateTimeOffset.UtcNow;
    }

    private static void UpdateOrderFromPaymentLink(PayOSOrder order, PaymentLink paymentLink)
    {
        order.Status = paymentLink.Status.ToString();
        order.Amount = paymentLink.Amount;
        order.AmountPaid = paymentLink.AmountPaid;
        order.AmountRemaining = paymentLink.AmountRemaining;
        order.CreatedAt = DateTimeOffset.TryParse(paymentLink.CreatedAt, out var createdAt)
            ? createdAt.UtcDateTime
            : order.CreatedAt;
        order.CanceledAt = DateTimeOffset.TryParse(paymentLink.CanceledAt, out var canceledAt)
            ? canceledAt.UtcDateTime
            : null;
        order.CancellationReason = paymentLink.CancellationReason;
    }

    private async Task ReplaceTransactionsFromPaymentLinkAsync(PayOSOrder order, PaymentLink paymentLink)
    {
        if (paymentLink.Transactions == null || paymentLink.Transactions.Count == 0)
        {
            return;
        }

        var existingTransactions = await _unitOfWork.GetRepository<PayOSTransaction>()
            .GetListAsync(predicate: t => t.OrderId == order.Id);
        if (existingTransactions.Any())
        {
            _unitOfWork.GetRepository<PayOSTransaction>().DeleteRangeAsync(existingTransactions);
        }

        var transactions = paymentLink.Transactions.Select(t => new PayOSTransaction
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Reference = t.Reference,
            Amount = t.Amount,
            AccountNumber = t.AccountNumber,
            Description = t.Description,
            TransactionDateTime = DateTimeOffset.TryParse(t.TransactionDateTime, out var transactionDateTime)
                ? transactionDateTime.UtcDateTime
                : DateTime.UtcNow,
            VirtualAccountName = t.VirtualAccountName,
            VirtualAccountNumber = t.VirtualAccountNumber,
            CounterAccountBankId = t.CounterAccountBankId,
            CounterAccountBankName = t.CounterAccountBankName,
            CounterAccountName = t.CounterAccountName,
            CounterAccountNumber = t.CounterAccountNumber
        }).ToList();

        await _unitOfWork.GetRepository<PayOSTransaction>().InsertRangeAsync(transactions);

        order.LastTransactionUpdate = DateTime.UtcNow;
        _unitOfWork.GetRepository<PayOSOrder>().UpdateAsync(order);
    }

    public async Task<PayOSOrderResponse?> GetPaymentByCallbackAsync(long orderCode, string? paymentLinkId)
    {
        var order = await _unitOfWork.Context.Set<PayOSOrder>()
            .Include(x => x.Items)
            .Include(x => x.Transactions)
            .Include(x => x.Invoices)
            .FirstOrDefaultAsync(x => x.OrderCode == orderCode
                || (!string.IsNullOrWhiteSpace(paymentLinkId) && x.PaymentLinkId == paymentLinkId));

        if (order == null)
        {
            return null;
        }

        var linkId = string.IsNullOrWhiteSpace(paymentLinkId) ? order.PaymentLinkId : paymentLinkId;
        if (!string.IsNullOrWhiteSpace(linkId))
        {
            var paymentLink = await _orderClient.PaymentRequests.GetAsync(linkId);
            UpdateOrderFromPaymentLink(order, paymentLink);
            await ReplaceTransactionsFromPaymentLinkAsync(order, paymentLink);
            await SyncDepositTransactionsToWalletAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }

        return MapOrderToResponse(order);
    }

    public async Task<WithdrawalResponse> CreateWithdrawalAsync(CreateWithdrawalRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var wallet = await _walletService.GetOrCreateWalletAsync(currentUserId.Value);

        if (request.Amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be greater than 0.", nameof(request.Amount));
        }

        if (wallet.Balance < request.Amount)
        {
            throw new InvalidOperationException("Insufficient wallet balance.");
        }

        var withdrawalId = Guid.NewGuid();
        
       
        var payoutRequest = new PayoutRequest
        {
            ReferenceId = withdrawalId.ToString("N"),
            Amount = (long)request.Amount,
            Description = NormalizeDescription(request.Description, $"AgroTemp RT {withdrawalId.ToString("N").Substring(28)}"),
            ToBin = ((int)request.ToBin).ToString(),
            ToAccountNumber = request.ToAccountNumber,
            Category = request.Category?.Any() == true ? request.Category : null
        };

        var payout = await _transferClient.Payouts.CreateAsync(payoutRequest);

        wallet.Balance -= request.Amount;
        wallet.UpdateAt = DateTime.UtcNow;
        _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);

        var withdrawalRequest = new WithdrawalRequest
        {
            Id = withdrawalId,
            WalletId = wallet.Id,
            Amount = request.Amount,
            BankAccountNumber = request.ToAccountNumber,
            BankName = string.Empty,// string.IsNullOrWhiteSpace(request.BankName) ? request.ToBin.ToString() : request.BankName,
            AccountHolderName = string.IsNullOrWhiteSpace(request.AccountHolderName)
                ? payout.Transactions.FirstOrDefault()?.ToAccountName ?? "Unknown"
                : request.AccountHolderName,
            Status = payout.ApprovalState.ToString(),
            Note = payout.Id,
            CreatedAt = DateTime.UtcNow,
            ProcessedAt = IsPayoutProcessed(payout) ? DateTime.UtcNow : null
        };

        await _unitOfWork.GetRepository<WithdrawalRequest>().InsertAsync(withdrawalRequest);

        await _unitOfWork.GetRepository<WalletTransaction>().InsertAsync(new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            JobDetailId = null,
            Type = TransactionType.WITHDRAW,
            Amount = request.Amount,
            BalanceAfter = wallet.Balance,
            ReferenceCode = payout.Id ?? withdrawalId.ToString(),
            Description = request.Description ?? "Withdraw from wallet",
            CreatedAt = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync();

        return MapWithdrawalToResponse(withdrawalRequest, payout);
    }

    public async Task<WithdrawalResponse?> GetWithdrawalAsync(Guid withdrawalId)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var withdrawal = await _unitOfWork.GetRepository<WithdrawalRequest>()
            .FirstOrDefaultAsync(
                predicate: wr => wr.Id == withdrawalId,
                include: q => q.Include(x => x.Wallet));

        if (withdrawal == null || withdrawal.Wallet.UserId != currentUserId.Value)
        {
            return null;
        }

        var payout = await GetPayoutByWithdrawalAsync(withdrawal);
        if (payout != null)
        {
            await ReconcileWithdrawalWalletAsync(withdrawal, payout);
            UpdateWithdrawalFromPayout(withdrawal, payout);
            _unitOfWork.GetRepository<WithdrawalRequest>().UpdateAsync(withdrawal);
            await _unitOfWork.SaveChangesAsync();
        }

        return MapWithdrawalToResponse(withdrawal, payout);
    }

    public async Task<ICollection<WithdrawalResponse>> GetMyWithdrawalsAsync()
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var withdrawals = await _unitOfWork.GetRepository<WithdrawalRequest>()
            .GetListAsync(
                predicate: wr => wr.Wallet.UserId == currentUserId.Value,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                include: q => q.Include(x => x.Wallet));

        var responses = new List<WithdrawalResponse>();
        foreach (var withdrawal in withdrawals)
        {
            responses.Add(MapWithdrawalToResponse(withdrawal, null));
        }

        return responses;
    }

    public async Task<WithdrawalAccountBalanceResponse> GetWithdrawalAccountBalanceAsync()
    {
        var accountInfo = await _transferClient.PayoutsAccount.GetBalanceAsync();
        var parsedBalance = long.TryParse(accountInfo.Balance, out var balance)
            ? balance
            : 0;

        return new WithdrawalAccountBalanceResponse
        {
            Balance = parsedBalance,
            AvailableBalance = parsedBalance,
            Currency = accountInfo.Currency
        };
    }

    public async Task<AgroTemp.Domain.DTO.Payment.AdminWalletStatsResponse> GetAdminWalletStatsAsync(DateTime? date = null)
    {
        var target = date ?? DateTime.UtcNow;
        var start = target.Date;
        var end = start.AddDays(1);

        var deposits = await _unitOfWork.GetRepository<WalletTransaction>()
            .GetListAsync(predicate: t => t.Type == TransactionType.DEPOSIT && t.CreatedAt >= start && t.CreatedAt < end);

        var withdraws = await _unitOfWork.GetRepository<WalletTransaction>()
            .GetListAsync(predicate: t => t.Type == TransactionType.WITHDRAW && t.CreatedAt >= start && t.CreatedAt < end);

        var depositAmount = deposits.Sum(d => d.Amount);
        var withdrawAmount = withdraws.Sum(w => w.Amount);
        var depositCount = deposits.Count;
        var withdrawCount = withdraws.Count;
        var totalTransactions = depositCount + withdrawCount;
        var netFlow = depositAmount - withdrawAmount;

        var wallets = await _unitOfWork.GetRepository<Domain.Entities.Wallet>().GetListAsync();
        var totalBalance = wallets.Any() ? wallets.Sum(w => w.Balance + w.LockedBalance) : 0m;
        var lockedBalance = wallets.Any() ? wallets.Sum(w => w.LockedBalance) : 0m;
        var availableBalance = wallets.Any() ? wallets.Sum(w => w.Balance) : 0m;

        return new AgroTemp.Domain.DTO.Payment.AdminWalletStatsResponse
        {
            SystemBalance = new AgroTemp.Domain.DTO.Payment.SystemBalanceDto
            {
                Total = totalBalance,
                Locked = lockedBalance,
                Available = availableBalance,
                ChangeToday = netFlow
            },
            PayosToday = new AgroTemp.Domain.DTO.Payment.PayosTodayDto
            {
                DepositAmount = depositAmount,
                WithdrawAmount = withdrawAmount,
                DepositCount = depositCount,
                WithdrawCount = withdrawCount,
                TotalTransactions = totalTransactions,
                NetFlow = netFlow
            }
        };
    }

    public async Task<AgroTemp.Domain.DTO.Payment.PaginatedAdminWithdrawalsResponse> GetWithdrawalsForAdminAsync(int page = 1, int limit = 20, string? status = null, string? search = null)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = 20;

        var query = _unitOfWork.Context.Set<Domain.Entities.WithdrawalRequest>()
            .Include(wr => wr.Wallet)
                .ThenInclude(w => w.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(wr => wr.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(wr => wr.Wallet.User.Email.ToLower().Contains(s) 
                || ( wr.Wallet.User.Worker != null && wr.Wallet.User.Worker.FullName.ToLower().Contains(s))
                || ( wr.Wallet.User.Farmer != null && wr.Wallet.User.Farmer.ContactName.ToLower().Contains(s)));
        }

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(wr => wr.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        var responses = items.Select(wr => MapWithdrawalToResponse(wr, null)).ToList();

        return new AgroTemp.Domain.DTO.Payment.PaginatedAdminWithdrawalsResponse
        {
            Items = responses,
            TotalCount = total,
            Page = page,
            Limit = limit
        };
    }

    // UpdateWithdrawalStatusAsync removed per admin PUT removal

    private async Task<PayOSOrder?> GetOrderEntityByIdAsync(Guid id)
    {
        return await _unitOfWork.Context.Set<PayOSOrder>()
            .Include(x => x.Items)
            .Include(x => x.Transactions)
            .Include(x => x.Invoices)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }

    private static PayOSOrderResponse MapOrderToResponse(PayOSOrder order)
    {
        return new PayOSOrderResponse
        {
            Id = order.Id,
            OrderCode = order.OrderCode,
            TotalAmount = order.TotalAmount,
            Description = order.Description,
            PaymentLinkId = order.PaymentLinkId,
            QrCode = order.QrCode,
            CheckoutUrl = order.CheckoutUrl,
            Status = order.Status,
            Amount = order.Amount,
            AmountPaid = order.AmountPaid,
            AmountRemaining = order.AmountRemaining,
            Bin = order.Bin,
            AccountNumber = order.AccountNumber,
            AccountName = order.AccountName,
            Currency = order.Currency,
            ReturnUrl = order.ReturnUrl,
            CancelUrl = order.CancelUrl,
            CreatedAt = ToOffset(order.CreatedAt),
            CanceledAt = ToOffset(order.CanceledAt),
            CancellationReason = order.CancellationReason,
            LastTransactionUpdate = ToOffset(order.LastTransactionUpdate),
            BuyerName = order.BuyerName,
            BuyerCompanyName = order.BuyerCompanyName,
            BuyerEmail = order.BuyerEmail,
            BuyerPhone = order.BuyerPhone,
            BuyerAddress = order.BuyerAddress,
            ExpiredAt = ToOffset(order.ExpiredAt),
            BuyerNotGetInvoice = order.BuyerNotGetInvoice,
            TaxPercentage = order.TaxPercentage,
            Items = order.Items.Select(i => new PayOSOrderItemResponse
            {
                Name = i.Name,
                Quantity = i.Quantity,
                Price = i.Price,
                Unit = i.Unit,
                TaxPercentage = i.TaxPercentage
            }).ToList(),
            Transactions = order.Transactions.Select(t => new PayOSOrderTransactionResponse
            {
                Reference = t.Reference,
                Amount = t.Amount,
                AccountNumber = t.AccountNumber,
                Description = t.Description,
                TransactionDateTime = ToOffset(t.TransactionDateTime) ?? DateTimeOffset.UtcNow,
                VirtualAccountName = t.VirtualAccountName,
                VirtualAccountNumber = t.VirtualAccountNumber,
                CounterAccountBankId = t.CounterAccountBankId,
                CounterAccountBankName = t.CounterAccountBankName,
                CounterAccountName = t.CounterAccountName,
                CounterAccountNumber = t.CounterAccountNumber
            }).ToList()
        };
    }

    private static PayOSInvoiceResponse MapInvoiceToResponse(PayOSInvoice invoice)
    {
        return new PayOSInvoiceResponse
        {
            InvoiceId = invoice.InvoiceId,
            InvoiceNumber = invoice.InvoiceNumber,
            IssuedTimestamp = invoice.IssuedTimestamp,
            IssuedDatetime = invoice.IssuedDatetime,
            TransactionId = invoice.TransactionId,
            ReservationCode = invoice.ReservationCode,
            CodeOfTax = invoice.CodeOfTax
        };
    }

    private static DateTimeOffset? ToOffset(DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        return new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc));
    }

    private static DateTimeOffset? ToOffset(DateTime value)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }

    private async Task CreditWalletFromPayOSAsync(PayOSOrder order, long amount, string? reference, string? description)
    {
        if (!order.UserId.HasValue || amount <= 0)
        {
            return;
        }

        var referenceCode = BuildDepositReferenceCode(order.OrderCode, reference);
        var existed = await _unitOfWork.GetRepository<WalletTransaction>()
            .FirstOrDefaultAsync(predicate: x => x.ReferenceCode == referenceCode);

        if (existed != null)
        {
            return;
        }

        var wallet = await _walletService.GetOrCreateWalletAsync(order.UserId.Value);
        wallet.Balance += amount;
        wallet.UpdateAt = DateTime.UtcNow;
        _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);

        await _unitOfWork.GetRepository<WalletTransaction>().InsertAsync(new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            JobDetailId = null,
            Type = TransactionType.DEPOSIT,
            Amount = amount,
            BalanceAfter = wallet.Balance,
            ReferenceCode = referenceCode,
            Description = string.IsNullOrWhiteSpace(description) ? "Deposit from PayOS" : description,
            CreatedAt = DateTime.UtcNow
        });
    }

    private async Task SyncDepositTransactionsToWalletAsync(PayOSOrder order)
    {
        if (order.Transactions == null || order.Transactions.Count == 0)
        {
            return;
        }

        foreach (var transaction in order.Transactions)
        {
            await CreditWalletFromPayOSAsync(order, transaction.Amount, transaction.Reference, transaction.Description);
        }
    }

    private async Task ReconcileWithdrawalWalletAsync(WithdrawalRequest withdrawal, Payout payout)
    {
        var payoutState = payout.ApprovalState.ToString().ToUpperInvariant();
        if (payoutState != "REJECTED")
        {
            return;
        }

        var refundReferenceCode = BuildWithdrawalRefundReferenceCode(withdrawal.Id);
        var existedRefund = await _unitOfWork.GetRepository<WalletTransaction>()
            .FirstOrDefaultAsync(predicate: x => x.ReferenceCode == refundReferenceCode);

        if (existedRefund != null)
        {
            return;
        }

        var wallet = await _unitOfWork.GetRepository<Wallet>()
            .FirstOrDefaultAsync(predicate: w => w.Id == withdrawal.WalletId);

        if (wallet == null)
        {
            return;
        }

        wallet.Balance += withdrawal.Amount;
        wallet.UpdateAt = DateTime.UtcNow;
        _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);

        await _unitOfWork.GetRepository<WalletTransaction>().InsertAsync(new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            JobDetailId = null,
            Type = TransactionType.REFUND,
            Amount = withdrawal.Amount,
            BalanceAfter = wallet.Balance,
            ReferenceCode = refundReferenceCode,
            Description = "Refund withdrawal amount because payout was rejected",
            CreatedAt = DateTime.UtcNow
        });
    }

    private static string BuildDepositReferenceCode(long orderCode, string? reference)
    {
        return string.IsNullOrWhiteSpace(reference)
            ? $"PAYOS-DEPOSIT-{orderCode}"
            : $"PAYOS-DEPOSIT-{orderCode}-{reference}";
    }

    private static string BuildWithdrawalRefundReferenceCode(Guid withdrawalId)
    {
        return $"PAYOS-WITHDRAW-REFUND-{withdrawalId:N}";
    }

    private async Task<Payout?> GetPayoutByWithdrawalAsync(WithdrawalRequest withdrawal)
    {
        if (!string.IsNullOrWhiteSpace(withdrawal.Note))
        {
            try
            {
                return await _transferClient.Payouts.GetAsync(withdrawal.Note);
            }
            catch
            {
                // Fall through to list by reference id.
            }
        }

        var payoutPage = await _transferClient.Payouts.ListAsync(new GetPayoutListParam { ReferenceId = withdrawal.Id.ToString() });
        return payoutPage.Data.FirstOrDefault();
    }

    private static bool IsPayoutProcessed(Payout payout)
    {
        var state = payout.ApprovalState.ToString().ToUpperInvariant();
        return state == "APPROVED" || state == "REJECTED" || state == "PAID";
    }

    private static void UpdateWithdrawalFromPayout(WithdrawalRequest withdrawal, Payout payout)
    {
        withdrawal.Status = payout.ApprovalState.ToString();
        withdrawal.Note = payout.Id;
        if (IsPayoutProcessed(payout))
        {
            withdrawal.ProcessedAt = DateTime.UtcNow;
        }
    }

    private static WithdrawalResponse MapWithdrawalToResponse(WithdrawalRequest withdrawal, Payout? payout)
    {
        var latestTx = payout?.Transactions?.FirstOrDefault();

        return new WithdrawalResponse
        {
            Id = withdrawal.Id,
            PayoutId = payout?.Id ?? withdrawal.Note,
            Amount = withdrawal.Amount,
            Status = withdrawal.Status,
            ApprovalState = payout?.ApprovalState.ToString(),
            BankName = withdrawal.BankName,
            AccountHolderName = withdrawal.AccountHolderName,
            BankAccountNumber = withdrawal.BankAccountNumber,
            ReferenceCode = latestTx?.Reference,
            Description = latestTx?.Description,
            CreatedAt = new DateTimeOffset(DateTime.SpecifyKind(withdrawal.CreatedAt, DateTimeKind.Utc)),
            ProcessedAt = withdrawal.ProcessedAt.HasValue
                ? new DateTimeOffset(DateTime.SpecifyKind(withdrawal.ProcessedAt.Value, DateTimeKind.Utc))
                : null
        };
    }
    private static string NormalizeDescription(string? description, string fallback)
    {
        var source = string.IsNullOrWhiteSpace(description) ? fallback : description;
        var normalizedString = source.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                if (char.IsLetterOrDigit(c) || c == ' ')
                {
                    stringBuilder.Append(c);
                }
            }
        }

        var result = stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC).Trim();
        return result.Length > 25 ? result.Substring(0, 25).Trim() : result;
    }

    private string BuildFrontendUrl(string relativePath)
    {
        var baseUrl = ResolveFrontendBaseUrl().TrimEnd('/');
        var path = relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";
        return $"{baseUrl}{path}";
    }

    private string ResolveFrontendBaseUrl()
    {
        // Prefer explicit configuration for deployment flexibility.
        var configuredBaseUrl =
            _configuration["PayOS:FrontendBaseUrl"]
            ?? _configuration["Frontend:BaseUrl"]
            ?? Environment.GetEnvironmentVariable("PAYOS_FRONTEND_BASE_URL")
            ?? Environment.GetEnvironmentVariable("FRONTEND_BASE_URL");

        if (!string.IsNullOrWhiteSpace(configuredBaseUrl))
        {
            return configuredBaseUrl;
        }

        var request = _httpContextAccessor.HttpContext?.Request;
        var origin = request?.Headers.Origin.ToString();
        if (Uri.TryCreate(origin, UriKind.Absolute, out var originUri)
            && IsSupportedLocalFrontend(originUri))
        {
            return originUri.GetLeftPart(UriPartial.Authority);
        }

        var referer = request?.Headers.Referer.ToString();
        if (Uri.TryCreate(referer, UriKind.Absolute, out var refererUri)
            && IsSupportedLocalFrontend(refererUri))
        {
            return refererUri.GetLeftPart(UriPartial.Authority);
        }

        var requestHost = request?.Host.Host;
        if (IsLocalHost(requestHost))
        {
            return DefaultLocalFrontendBaseUrl;
        }

        var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
        if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
        {
            return DefaultLocalFrontendBaseUrl;
        }

        return "https://www.agrotemp.dev";
    }

    private static bool IsSupportedLocalFrontend(Uri uri)
    {
        return IsLocalHost(uri.Host) && uri.Port > 0 && LocalFrontendPorts.Contains(uri.Port);
    }

    private static bool IsLocalHost(string? host)
    {
        return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
            || string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(host, "::1", StringComparison.OrdinalIgnoreCase);
    }
}
