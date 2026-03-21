using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Payment;
using AgroTemp.Domain.Entities;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.V2.PaymentRequests.Invoices;
using PayOS.Models.Webhooks;
using System.Security.Claims;

namespace AgroTemp.Service.Implements;

public class PayOSService : IPayOSService
{
    private readonly PayOSClient _client;
    private readonly IUnitOfWork<AgroTempDbContext> _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PayOSService(
        PayOSClient client,
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PayOSOrderResponse?> GetOrderAsync(Guid id)
    {
        var order = await GetOrderEntityByIdAsync(id);
        if (order == null || string.IsNullOrWhiteSpace(order.PaymentLinkId))
        {
            return null;
        }

        var paymentLink = await _client.PaymentRequests.GetAsync(order.PaymentLinkId);
        UpdateOrderFromPaymentLink(order, paymentLink);
        await ReplaceTransactionsFromPaymentLinkAsync(order, paymentLink);
        await _unitOfWork.SaveChangesAsync();

        return MapOrderToResponse(order);
    }

    public async Task<PayOSOrderResponse> CreatePaymentAsync(CreatePayOSOrderRequest request)
    {
        var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var returnUrl = request.ReturnUrl ?? "https://your-domain.com/success";
        var cancelUrl = request.CancelUrl ?? "https://your-domain.com/cancel";

        var paymentRequest = new CreatePaymentLinkRequest
        {
            OrderCode = orderCode,
            Amount = request.TotalAmount,
            Description = request.Description ?? $"order {orderCode}",
            ReturnUrl = returnUrl,
            CancelUrl = cancelUrl,
            BuyerName = request.BuyerName,
            BuyerCompanyName = request.BuyerCompanyName,
            BuyerEmail = request.BuyerEmail,
            BuyerPhone = request.BuyerPhone,
            BuyerAddress = request.BuyerAddress,
            ExpiredAt = request.ExpiredAt?.ToUnixTimeSeconds(),
            Items = request.Items.Select(i => new PaymentLinkItem
            {
                Name = i.Name ?? string.Empty,
                Quantity = i.Quantity,
                Price = i.Price,
                Unit = i.Unit,
                TaxPercentage = i.TaxPercentage.HasValue ? (TaxPercentage?)i.TaxPercentage.Value : null
            }).ToList()
        };

        if (request.BuyerNotGetInvoice.HasValue || request.TaxPercentage.HasValue)
        {
            paymentRequest.Invoice = new InvoiceRequest
            {
                BuyerNotGetInvoice = request.BuyerNotGetInvoice,
                TaxPercentage = request.TaxPercentage.HasValue ? (TaxPercentage?)request.TaxPercentage.Value : null
            };
        }

        var paymentResponse = await _client.PaymentRequests.CreateAsync(paymentRequest);

        var order = new PayOSOrder
        {
            Id = Guid.NewGuid(),
            UserId = GetCurrentUserId(),
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
            BuyerName = request.BuyerName,
            BuyerCompanyName = request.BuyerCompanyName,
            BuyerEmail = request.BuyerEmail,
            BuyerPhone = request.BuyerPhone,
            BuyerAddress = request.BuyerAddress,
            ExpiredAt = request.ExpiredAt?.UtcDateTime,
            BuyerNotGetInvoice = request.BuyerNotGetInvoice,
            TaxPercentage = request.TaxPercentage,
            Items = request.Items.Select(i => new PayOSOrderItem
            {
                Id = Guid.NewGuid(),
                Name = i.Name,
                Quantity = i.Quantity,
                Price = i.Price,
                Unit = i.Unit,
                TaxPercentage = i.TaxPercentage
            }).ToList()
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

        var paymentLink = await _client.PaymentRequests.CancelAsync(order.PaymentLinkId, cancellationReason ?? "Cancelled by user");
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

        var invoices = await _client.PaymentRequests.Invoices.GetAsync(order.PaymentLinkId);

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

        var invoiceFile = await _client.PaymentRequests.Invoices.DownloadAsync(invoiceId, order.PaymentLinkId);
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
        var webhookData = await _client.Webhooks.VerifyAsync(webhook);
        if (webhookData.OrderCode == 123 && webhookData.Description == "VQRIO123" && webhookData.AccountNumber == "12345678")
        {
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
            }

            var transactions = await _unitOfWork.GetRepository<PayOSTransaction>()
                .GetListAsync(predicate: x => x.OrderId == order.Id);

            order.AmountPaid = transactions.Sum(x => x.Amount);
            order.AmountRemaining = order.Amount - order.AmountPaid;
            order.Status = order.AmountRemaining > 0 ? PaymentLinkStatus.Underpaid.ToString() : PaymentLinkStatus.Paid.ToString();
            order.LastTransactionUpdate = DateTime.UtcNow;

            _unitOfWork.GetRepository<PayOSOrder>().UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }

        return new PayOSWebhookResultResponse
        {
            Message = "Webhook processed successfully",
            OrderCode = webhookData.OrderCode
        };
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
}
