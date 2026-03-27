using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Skill;

public class UpdateSkillRequest
{
    [Required]
    [StringLength(256)]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; }
}