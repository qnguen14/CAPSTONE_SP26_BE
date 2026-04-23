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

    /// <summary>
    /// Optional. When set by Admin, only that user (and Admin) can see this message.
    /// Null = visible to all parties (legacy / public comment).
    /// </summary>
    [Column("target_user_id")]
    public Guid? TargetUserId { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; }

    [Column("attachment_url")]
    public string? AttachmentUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
