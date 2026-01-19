using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum WageType
{
    Hourly = 1,
    Daily = 2,
    PerJob = 3
}

public enum PaymentMethod
{
    Cash = 1,
    BankTransfer = 2,
    EWallet = 3,
    EscrowService = 4
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
    [ForeignKey(nameof(FarmerProfile))]
    [Column("farmer_profile_id")]
    public Guid FarmerProfileId { get; set; }
    public virtual FarmerProfile FarmerProfile { get; set; }

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

    [Required]
    [Column("latitude")]
    public decimal Latitude { get; set; }

    [Required]
    [Column("longitude")]
    public decimal Longitude { get; set; }

    [Required]
    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Required]
    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [Required]
    [Column("estimated_hours")]
    public decimal EstimatedHours { get; set; }

    [Required]
    [Column("workers_needed")]
    public int WorkersNeeded { get; set; }

    [Required]
    [Column("workers_accepted")]
    public int WorkersAccepted { get; set; }

    [Required]
    [Column("wage_type")]
    public int WageTypeId { get; set; }
    public virtual WageType WageType { get; set; }

    [Required]
    [Column("wage_amount")]
    public decimal WageAmount { get; set; }

    [Required]
    [Column("payment_method")]
    public int PaymentMethodId { get; set; }
    public virtual PaymentMethod PaymentMethod { get; set; }

    [Required]
    [Column("required_skills")]
    public string RequiredSkills { get; set; }

    [Required]
    [Column("gender_preference")]
    public string GenderPreference { get; set; }

    [Required]
    [Column("age_requirement")]
    public string AgeRequirement { get; set; }

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
    public virtual JobPostStatus Status { get; set; }
}
