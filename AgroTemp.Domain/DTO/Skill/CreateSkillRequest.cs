using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Skill;

public class CreateSkillRequest
{
    [Required]
    [StringLength(256)]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}