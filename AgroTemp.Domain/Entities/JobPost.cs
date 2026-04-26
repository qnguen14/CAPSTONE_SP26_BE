using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum JobType
{
    PerJob = 1,
    Daily = 2
}

public enum JobPostStatus
{
    Draft = 1,
    Published = 2,
    Closed = 3,
    InProgress = 4,
    Completed = 5,
    Cancelled = 6
}

[Table("Job_Post")]
public class JobPost
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Farmer))]
    [Column("farmer_id")]
    public Guid FarmerId { get; set; }
    public virtual Farmer Farmer { get; set; }

    [Required]
    [ForeignKey(nameof(Farm))]
    [Column("farm_id")]
    public Guid FarmId { get; set; }
    public virtual Farm Farm { get; set; }

    [Required]
    [ForeignKey(nameof(JobCategory))]
    [Column("job_category_id")]
    public Guid JobCategoryId { get; set; }
    public virtual JobCategory JobCategory { get; set; }

    [Required]
    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; }

    [Required]
    [Column("description")]
    public string Description { get; set; }

    [Required]
    [Column("address")]
    public string Address { get; set; }

    [Column("start_date")]
    public DateOnly? StartDate { get; set; }

    [Column("end_date")]
    public DateOnly? EndDate { get; set; }

    [Column("selected_days")]
    public List<DateOnly> SelectedDays { get; set; } = new List<DateOnly>();

    [Column("start_time")]
    public TimeOnly StartTime { get; set; }
    
    [Column("end_time")]
    public TimeOnly EndTime { get; set; }

    [Column("workers_needed")]
    public int WorkersNeeded { get; set; }

    [Required]
    [Column("workers_accepted")]
    public int WorkersAccepted { get; set; }

    [Required]
    [Column("job_type")]
    public int JobTypeId { get; set; }

    [Required]
    [Column("wage_amount")]
    public decimal WageAmount { get; set; }

    [Column("requirements")]
    public List<string>? Requirements { get; set; }

    [Column("privileges")]
    public List<string>? Privileges { get; set; }

    [Required]
    [Column("published_at")]
    public DateTime PublishedAt { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Required]
    [Column("is_urgent")]
    public bool IsUrgent { get; set; }

    [Required]
    [Column("status")]
    public int StatusId { get; set; }

    public virtual ICollection<JobSkillRequirement> JobSkillRequirements { get; set; } = new List<JobSkillRequirement>();
    public virtual ICollection<JobDetail> JobDetails { get; set; } = new List<JobDetail>();
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    public virtual ICollection<SavedJobPost> SavedJobPosts { get; set; } = new List<SavedJobPost>();

}
