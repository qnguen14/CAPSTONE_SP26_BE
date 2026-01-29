using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum ProficiencyLevel
{
    Beginner = 1,
    Intermediate = 2,
    Experienced = 3
}

[Table("Worker_Skill")]
public class WorkerSkill
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(WorkerProfile))]
    [Column("worker_profile_id")]
    public Guid WorkerProfileId { get; set; }
    public virtual WorkerProfile WorkerProfile { get; set; }

    [Required]
    [ForeignKey(nameof(Skill))]
    [Column("skill_id")]
    public Guid SkillId { get; set; }
    public virtual Skill Skill { get; set; }

    [Required]
    [Column("proficiency_level")]
    public int ProficiencyLevelId { get; set; }

    [NotMapped]
    public ProficiencyLevel ProficiencyLevel
    {
        get => (ProficiencyLevel)ProficiencyLevelId;
        set => ProficiencyLevelId = (int)value;
    }

    [Required]
    [Column("years_experience")]
    public int YearsExperience { get; set; }
}
