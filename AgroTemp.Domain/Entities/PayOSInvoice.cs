using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("PayOS_Invoice")]
public class PayOSInvoice
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Order))]
    [Column("order_id")]
    public Guid OrderId { get; set; }
    public virtual PayOSOrder Order { get; set; }

    [Column("invoice_id")]
    [StringLength(128)]
    public string? InvoiceId { get; set; }

    [Column("invoice_number")]
    [StringLength(128)]
    public string? InvoiceNumber { get; set; }

    [Column("issued_timestamp")]
    public long? IssuedTimestamp { get; set; }

    [Column("issued_datetime")]
    public DateTime? IssuedDatetime { get; set; }

    [Column("transaction_id")]
    [StringLength(128)]
    public string? TransactionId { get; set; }

    [Column("reservation_code")]
    [StringLength(128)]
    public string? ReservationCode { get; set; }

    [Column("code_of_tax")]
    [StringLength(128)]
    public string? CodeOfTax { get; set; }
}
