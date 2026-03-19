namespace AgroTemp.Domain.DTO.Payment;

public class PayOSInvoicesInfoResponse
{
    public List<PayOSInvoiceResponse> Invoices { get; set; } = new();
}

public class PayOSInvoiceResponse
{
    public string? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public long? IssuedTimestamp { get; set; }
    public DateTime? IssuedDatetime { get; set; }
    public string? TransactionId { get; set; }
    public string? ReservationCode { get; set; }
    public string? CodeOfTax { get; set; }
}

public class PayOSWebhookResultResponse
{
    public string Message { get; set; } = string.Empty;
    public long? OrderCode { get; set; }
}
