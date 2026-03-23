using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO;

public class UpdateFarmerProfileRequest
{
    [Required]
    [StringLength(256)]
    public string ContactName { get; set; }

    [Required]
    public string AvatarUrl { get; set; }
}
