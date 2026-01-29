using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Job_Skill_Requirement")]
public class JobSkillRequirement
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(JobPost))]
    [Column("job_post_id")]
    public Guid JobPostId { get; set; }
    public virtual JobPost JobPost { get; set; }

    [Required]
    [ForeignKey(nameof(Skill))]
    [Column("skill_id")]
    public Guid SkillId { get; set; }
    public virtual Skill Skill { get; set; }

    [Required]
    [Column("required_level")]
    public int RequiredLevelId { get; set; }

    [NotMapped]
    public ProficiencyLevel RequiredLevel
    {
        get => (ProficiencyLevel)RequiredLevelId;
        set => RequiredLevelId = (int)value;
    }

    [Required]
    [Column("is_mandatory")]
    public bool IsMandatory { get; set; }
}
