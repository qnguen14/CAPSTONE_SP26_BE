using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum ExperienceLevel
{
    Beginner = 1,
    Intermediate = 2,
    Experienced = 3
}

public enum Gender
{
    Male = 1,
    Female = 2
}

[Table("Worker")]
public class Worker
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    [Column("user_id")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    [Required]
    [Column("full_name")]
    [StringLength(256)]
    public string FullName { get; set; }

    [Required]
    [Column("date_of_birth")]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    [Column("primary_location")]
    public string PrimaryLocation { get; set; }

    [Column("travel_radius_km_preference")]
    public double? TravelRadiusKmPreference { get; set; }

    [Required]
    [Column("experience_level")]
    public int ExperienceLevelId { get; set; }
    
    [NotMapped]
    public ExperienceLevel ExperienceLevel 
    { 
        get => (ExperienceLevel)ExperienceLevelId;
        set => ExperienceLevelId = (int)value;
    }

    [Required]
    [Column("average_rating")]
    public decimal AverageRating { get; set; }

    [Required]
    [Column("availability_schedule")]
    public string AvailabilitySchedule { get; set; }

    [Required]
    [Column("total_jobs_completed")]
    public int TotalJobsCompleted { get; set; }

    public virtual ICollection<WorkerSkill> WorkerSkills { get; set; } = new List<WorkerSkill>();
    public virtual ICollection<JobDetail> JobDetails { get; set; } = new List<JobDetail>();

    [Required]
    [Column("avatar_url")]
    public string AvatarUrl { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Required]
    [Column("gender")]
    public int GenderId { get; set; }
    
    [NotMapped]
    public Gender Gender 
    { 
        get => (Gender)GenderId;
        set => GenderId = (int)value;
    }
}
