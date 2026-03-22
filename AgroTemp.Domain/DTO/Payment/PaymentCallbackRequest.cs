namespace AgroTemp.Domain.DTO.Payment;

public class PaymentCallbackRequest
{
    /// <summary>
    /// PayOS result code. "00" means success.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// PayOS payment link ID (e.g. d271359660a4464d9748c5d7f0f05230).
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// True if the user cancelled the payment.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Payment status returned by PayOS (e.g. PAID, CANCELLED, PENDING).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// The order code used when creating the payment link.
    /// </summary>
    public long OrderCode { get; set; }
}
