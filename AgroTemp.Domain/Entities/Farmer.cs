using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Farmer")]
public class Farmer
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
    [Column("contact_name")]
    [StringLength(256)]
    public string ContactName { get; set; }

    [Required]
    [Column("contact_number")]
    [StringLength(15)]
    public string ContactNumber { get; set; }

    [Required]
    [Column("average_rating")]
    public decimal AverageRating { get; set; }

    [Required]
    [Column("total_jobs_posted")]
    public int TotalJobsPosted { get; set; }

    [Required]
    [Column("total_jobs_completed")]
    public int TotalJobsCompleted { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Required]
    [Column("avatar_url")]
    public string AvatarUrl { get; set; }
    // Navigation property for farms owned by this farmer
    public virtual ICollection<Farm> Farms { get; set; } = new List<Farm>();
}
