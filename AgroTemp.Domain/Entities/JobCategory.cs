using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Job_Category")]
public class JobCategory
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(256)]
    public string Name { get; set; }

    [Required]
    [Column("description")]
    public string Description { get; set; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; }


    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public virtual ICollection<Farm> Farms { get; set; } = new List<Farm>();
}
