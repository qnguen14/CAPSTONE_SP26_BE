using AgroTemp.Domain.DTO.Payment;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;

namespace AgroTemp.Service.Interfaces;

public interface IPayOSService
{
    Task<PayOSOrderResponse?> GetOrderAsync(Guid id);
    Task<PayOSOrderResponse> CreatePaymentAsync(CreatePayOSOrderRequest request);
    Task<PaymentLink?> CancelPaymentAsync(Guid orderId, string? cancellationReason);
    Task<PayOSInvoicesInfoResponse?> GetInvoicesAsync(Guid orderId);
    Task<(byte[] Content, string ContentType, string FileName)?> DownloadInvoiceAsync(Guid orderId, string invoiceId);
    Task<PayOSWebhookResultResponse> VerifyWebhookAsync(Webhook webhook);
    Task<PayOSOrderResponse?> GetPaymentByCallbackAsync(long orderCode, string? paymentLinkId);
    Task<WithdrawalResponse> CreateWithdrawalAsync(CreateWithdrawalRequest request);
    Task<WithdrawalResponse?> GetWithdrawalAsync(Guid withdrawalId);
    Task<ICollection<WithdrawalResponse>> GetMyWithdrawalsAsync();
    Task<WithdrawalAccountBalanceResponse> GetWithdrawalAccountBalanceAsync();
    Task<AgroTemp.Domain.DTO.Payment.AdminWalletStatsResponse> GetAdminWalletStatsAsync(DateTime? date = null);
    Task<AgroTemp.Domain.DTO.Payment.PaginatedAdminWithdrawalsResponse> GetWithdrawalsForAdminAsync(int page = 1, int limit = 20, string? status = null, string? search = null);
    // UpdateWithdrawalStatusAsync removed per admin PUT removal
}
