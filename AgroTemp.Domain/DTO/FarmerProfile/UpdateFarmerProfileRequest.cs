using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO;

public class UpdateFarmerProfileRequest
{
    [Required]
    [StringLength(256)]
    public string ContactName { get; set; }

    public string AvatarUrl { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public DateOnly DateOfBirth { get; set; }
}
