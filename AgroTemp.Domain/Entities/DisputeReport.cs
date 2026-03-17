using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum DisputeStatus
{
    Pending = 1,
    UnderReview = 2,
    Resolved = 3,
    Rejected = 4
}

public enum DisputeType
{
    JobQuality = 1,
    Payment = 2,
    Other = 3
}

[Table("dispute_reports")]
public class DisputeReport
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("farmer_id")]
    [ForeignKey(nameof(Farmer))]
    public Guid? FarmerId { get; set; }
    public virtual Farmer Farmer { get; set; }

    [Column("worker_id")]
    [ForeignKey(nameof(Worker))]
    public Guid? WorkerId { get; set; }
    public virtual Worker Worker { get; set; }

    [Required]
    [Column("job_post_id")]
    [ForeignKey(nameof(JobPost))]
    public Guid JobPostId { get; set; }
    public virtual JobPost JobPost { get; set; }

    [Required]
    [Column("dispute_type")]
    public int DisputeTypeId { get; set; }

    [NotMapped]
    public DisputeType DisputeType
    {
        get => (DisputeType)DisputeTypeId;
        set => DisputeTypeId = (int)value;
    }

    [Required]
    [Column("reason")]
    [StringLength(512)]
    public string Reason { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("evidence_url")]
    public string? EvidenceUrl { get; set; }

    [Required]
    [Column("status")]
    public int StatusId { get; set; } = (int)DisputeStatus.Pending;

    [NotMapped]
    public DisputeStatus Status
    {
        get => (DisputeStatus)StatusId;
        set => StatusId = (int)value;
    }

    [Column("admin_note")]
    public string? AdminNote { get; set; }

    [Column("resolved_by_id")]
    [ForeignKey(nameof(ResolvedBy))]
    public Guid? ResolvedById { get; set; }
    public virtual User ResolvedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("resolved_at")]
    public DateTime? ResolvedAt { get; set; }
}