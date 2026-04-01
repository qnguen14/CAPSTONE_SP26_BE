using AgroTemp.Domain.DTO.Skill;

namespace AgroTemp.Domain.DTO;

public class WorkerProfileDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Date_of_birth { get; set; }
    public string PrimaryLocation { get; set; }
    public double? TravelRadiusKmPreference { get; set; }
    public int ExperienceLevelId { get; set; }
    public string ExperienceLevel { get; set; }
    public decimal AverageRating { get; set; }
    public string AvailabilitySchedule { get; set; }
    public int TotalJobsCompleted { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public List<SkillResponse> Skills { get; set; } = new List<SkillResponse>();
}
