using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Dispute_Report_Comments")]
public class DisputeReportComment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("dispute_report_id")]
    [ForeignKey(nameof(DisputeReport))]
    public Guid DisputeReportId { get; set; }
    public virtual DisputeReport DisputeReport { get; set; }

    [Required]
    [Column("user_id")]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; }

    [Column("attachment_url")]
    public string? AttachmentUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
