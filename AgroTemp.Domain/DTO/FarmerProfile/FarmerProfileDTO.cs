namespace AgroTemp.Domain.DTO;

public class FarmerProfileDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ContactName { get; set; }
    public string Address { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public Guid MainFarmId { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalJobsPosted { get; set; }
    public int TotalJobsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string AvatarUrl { get; set; }
    public UserDTO User { get; set; }
}
