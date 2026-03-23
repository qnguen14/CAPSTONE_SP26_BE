using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO;

public class UpdateFarmerProfileRequest
{
    [Required]
    [StringLength(256)]
    public string OrganizationName { get; set; }

    [Required]
    [StringLength(256)]
    public string ContactName { get; set; }

    [StringLength(256)]
    public string? CooperativeAffiliation { get; set; }

    [Required]
    public string AvatarUrl { get; set; }
}
