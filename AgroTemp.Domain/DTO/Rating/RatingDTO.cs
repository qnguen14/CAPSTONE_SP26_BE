using AgroTemp.Domain.DTO;

namespace AgroTemp.Domain.DTO.Rating
{
    public class RatingUserProfileDTO
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public FarmerProfileDTO? FarmerProfile { get; set; }
        public WorkerProfileDTO? WorkerProfile { get; set; }
    }

    public class RatingDTO
    {
        public Guid Id { get; set; }
        public Guid RaterId { get; set; }
        public Guid RateeId { get; set; }
        public Guid JobPostId { get; set; }
        public int RatingScore { get; set; }
        public string? ReviewText { get; set; }
        public int TypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public RatingUserProfileDTO? RaterProfile { get; set; }
        public RatingUserProfileDTO? RateeProfile { get; set; }
    }
}
