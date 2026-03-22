using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("PayOS_Webhook_Log")]
public class PayOSWebhookLog
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("order_id")]
    public Guid? OrderId { get; set; }

    [Column("order_code")]
    public long? OrderCode { get; set; }

    [Column("reference")]
    [StringLength(128)]
    public string? Reference { get; set; }

    [Required]
    [Column("raw_payload")]
    public string RawPayload { get; set; } = string.Empty;

    [Column("is_payload_parsed")]
    public bool IsPayloadParsed { get; set; }

    [Column("is_verified")]
    public bool IsVerified { get; set; }

    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    [Required]
    [Column("received_at")]
    public DateTime ReceivedAt { get; set; }

    [Column("processed_at")]
    public DateTime? ProcessedAt { get; set; }
}