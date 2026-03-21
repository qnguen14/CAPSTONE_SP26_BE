using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum JobStatus
{
    InProgress = 1,
    Completed = 2,
    CancelledByFarmer = 3,
    CancelledByWorker = 4
}

[Table("Job_Detail")]
public class JobDetail
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(JobApplication))]
    [Column("job_application_id")]
    public Guid JobApplicationId { get; set; }
    public virtual JobApplication JobApplication { get; set; }

    [Required]
    [ForeignKey(nameof(JobPost))]
    [Column("job_post_id")]
    public Guid JobPostId { get; set; }
    public virtual JobPost JobPost { get; set; }

    [Required]
    [ForeignKey(nameof(Worker))]
    [Column("worker_id")]
    public Guid WorkerId { get; set; }
    public virtual Worker Worker { get; set; }

    [Required]
    [Column("status")]
    public int StatusId { get; set; }

    [NotMapped]
    public JobStatus Status
    {
        get => (JobStatus)StatusId;
        set => StatusId = (int)value;
    }

    [Column("work_date")]
    public DateTime? WorkDate { get; set; }

    [Column("worker_description")]
    public string WorkerDescription { get; set; }
    [Column("confirmed_at")]
    public DateTime? ConfirmedAt { get; set; }

    [Column("farmer_feedback")]
    public string FarmerFeedback { get; set; }

    [Column("farmer_approved_percent")]
    public int? FarmerApprovedPercent { get; set; }

    [Column("job_price", TypeName = "decimal(18,2)")]
    public decimal JobPrice { get; set; }

    [Column("worker_payment_amount", TypeName = "decimal(18,2)")]
    public decimal? WorkerPaymentAmount { get; set; }

    [Column("refund_amount", TypeName = "decimal(18,2)")]
    public decimal? RefundAmount { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<WorkerSession> WorkerSessions { get; set; } = new List<WorkerSession>();
    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    //public ICollection<JobAttachment> JobAttachments { get; set; } = new List<JobAttachment>();
}
