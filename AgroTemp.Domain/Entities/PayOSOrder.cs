using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("PayOS_Order")]
public class PayOSOrder
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Required]
    [Column("order_code")]
    public long OrderCode { get; set; }

    [Required]
    [Column("total_amount")]
    public long TotalAmount { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("payment_link_id")]
    [StringLength(128)]
    public string? PaymentLinkId { get; set; }

    [Column("qr_code")]
    public string? QrCode { get; set; }

    [Column("checkout_url")]
    public string? CheckoutUrl { get; set; }

    [Column("status")]
    [StringLength(64)]
    public string? Status { get; set; }

    [Required]
    [Column("amount")]
    public long Amount { get; set; }

    [Required]
    [Column("amount_paid")]
    public long AmountPaid { get; set; }

    [Required]
    [Column("amount_remaining")]
    public long AmountRemaining { get; set; }

    [Column("bin")]
    [StringLength(32)]
    public string? Bin { get; set; }

    [Column("account_number")]
    [StringLength(64)]
    public string? AccountNumber { get; set; }

    [Column("account_name")]
    [StringLength(256)]
    public string? AccountName { get; set; }

    [Column("currency")]
    [StringLength(16)]
    public string? Currency { get; set; }

    [Column("return_url")]
    public string? ReturnUrl { get; set; }

    [Column("cancel_url")]
    public string? CancelUrl { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("canceled_at")]
    public DateTime? CanceledAt { get; set; }

    [Column("cancellation_reason")]
    public string? CancellationReason { get; set; }

    [Column("last_transaction_update")]
    public DateTime? LastTransactionUpdate { get; set; }

    [Column("buyer_name")]
    [StringLength(256)]
    public string? BuyerName { get; set; }

    [Column("buyer_company_name")]
    [StringLength(256)]
    public string? BuyerCompanyName { get; set; }

    [Column("buyer_email")]
    [StringLength(256)]
    public string? BuyerEmail { get; set; }

    [Column("buyer_phone")]
    [StringLength(64)]
    public string? BuyerPhone { get; set; }

    [Column("buyer_address")]
    public string? BuyerAddress { get; set; }

    [Column("expired_at")]
    public DateTime? ExpiredAt { get; set; }

    [Column("buyer_not_get_invoice")]
    public bool? BuyerNotGetInvoice { get; set; }

    [Column("tax_percentage")]
    public int? TaxPercentage { get; set; }

    public virtual ICollection<PayOSOrderItem> Items { get; set; } = new List<PayOSOrderItem>();
    public virtual ICollection<PayOSTransaction> Transactions { get; set; } = new List<PayOSTransaction>();
    public virtual ICollection<PayOSInvoice> Invoices { get; set; } = new List<PayOSInvoice>();
}
