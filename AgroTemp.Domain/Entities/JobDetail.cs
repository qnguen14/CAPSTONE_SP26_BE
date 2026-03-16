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

    [Column("confirmed_at")]
    public DateTime? ConfirmedAt { get; set; }

    [Column("started_at")]
    public DateTime? StartedAt { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("worker_checked_in")]
    public bool WorkerCheckedIn { get; set; }

    [Column("farmer_confirmed_attendance")]
    public bool FarmerConfirmedAttendance { get; set; }

    [Column("total_hours_worked")]
    public decimal? TotalHoursWorked { get; set; }

    [Column("total_amount_due")]
    public decimal? TotalAmountDue { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<WorkerSession> WorkerSessions { get; set; } = new List<WorkerSession>();

}
