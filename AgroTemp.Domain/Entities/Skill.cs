using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Skill")]
public class Skill
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
    [Column("job_category_id")]
    public Guid JobCategoryId { get; set; }
    public virtual JobCategory? Category { get; set; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; }
    public virtual ICollection<WorkerSkill> WorkerSkills { get; set; } = new List<WorkerSkill>();
    public virtual ICollection<JobSkillRequirement> JobSkillRequirements { get; set; } = new List<JobSkillRequirement>();
}
