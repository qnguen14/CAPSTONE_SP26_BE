using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum SkillCategory
{
    Agronomy = 1,
    AnimalHusbandry = 2,
    Aquiculture = 3
}

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
    
    public virtual ICollection<WorkerSkill> WorkerSkills { get; set; } = new List<WorkerSkill>();
    public virtual ICollection<JobSkillRequirement> JobSkillRequirements { get; set; } = new List<JobSkillRequirement>();
    
    [Required]
    [Column("description")]
    public string Description { get; set; }

    [Required]
    [Column("category")]
    public int CategoryId { get; set; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; }
}
