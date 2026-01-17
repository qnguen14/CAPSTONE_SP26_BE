using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO;

public class UpdateWorkerProfileRequest
{
    [Required]
    [StringLength(256)]
    public string FullName { get; set; }
    
    [Required]
    [StringLength(50)]
    public string AgeRange { get; set; }
    
    [Required]
    public string PrimaryLocation { get; set; }
    
    public double? TravelRadiusKmPreference { get; set; }
    
    [Required]
    [Range(1, 3)]
    public int ExperienceLevelId { get; set; }
    
    [Required]
    public string AvailabilitySchedule { get; set; }
    
    [Required]
    public string AvatarUrl { get; set; }
}
