using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Saved_Job_Post")]
public class SavedJobPost
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Worker))]
    [Column("worker_id")]
    public Guid WorkerId { get; set; }
    public virtual Worker Worker { get; set; }

    [Required]
    [ForeignKey(nameof(JobPost))]
    [Column("job_post_id")]
    public Guid JobPostId { get; set; }
    public virtual JobPost JobPost { get; set; }

    [Required]
    [Column("saved_at")]
    public DateTime SavedAt { get; set; }
}
