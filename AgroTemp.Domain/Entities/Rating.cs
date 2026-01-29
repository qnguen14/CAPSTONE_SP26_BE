using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum RatingType
{
    FarmerToWorker = 1,
    WorkerToFarmer = 2
}

[Table("Rating")]
public class Rating
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Rater))]
    [Column("rater_id")]
    public Guid RaterId { get; set; }
    public virtual User Rater { get; set; }

    [Required]
    [ForeignKey(nameof(Ratee))]
    [Column("ratee_id")]
    public Guid RateeId { get; set; }
    public virtual User Ratee { get; set; }

    [Required]
    [ForeignKey(nameof(JobPost))]
    [Column("job_id")]
    public Guid JobPostId { get; set; }
    public virtual JobPost JobPost { get; set; }

    [Required]
    [Column("rating_score")]
    [Range(1, 5)]
    public int RatingScore { get; set; }

    [Column("review_text")]
    public string? ReviewText { get; set; }

    [Required]
    [Column("type")]
    public int TypeId { get; set; }

    [NotMapped]
    public RatingType Type
    {
        get => (RatingType)TypeId;
        set => TypeId = (int)value;
    }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
