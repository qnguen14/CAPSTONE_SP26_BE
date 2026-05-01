using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Job_Post_Day")]
public class JobPostDay
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(JobPost))]
    [Column("job_post_id")]
    public Guid JobPostId { get; set; }
    public virtual JobPost JobPost { get; set; }

    [Required]
    [Column("work_date")]
    public DateOnly WorkDate { get; set; }

    [Required]
    [Column("workers_needed")]
    public int WorkersNeeded { get; set; }

    [Required]
    [Column("workers_accepted")]
    public int WorkersAccepted { get; set; }
}
