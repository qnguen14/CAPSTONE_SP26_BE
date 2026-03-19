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
}
