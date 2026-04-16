using AgroTemp.Domain.DTO.Rating;

namespace AgroTemp.Service.Interfaces
{
    public interface IRatingService
    {
        Task<List<RatingDTO>> GetAllRatings();
        Task<RatingDTO> GetRatingById(Guid id);
        Task<RatingDTO> CreateRating(CreateRatingRequest request);
        Task<RatingDTO> UpdateRating(Guid id, UpdateRatingRequest request);
        Task<bool> DeleteRating(Guid id);
        Task<RatingDTO> GetSpecificRatingByUserId(Guid userId);
        Task<List<RatingDTO>> GetAllRatingsByUserId(Guid userId);
        Task<List<RatingDTO>> GetGivenRatingsByUser();
        Task<List<RatingDTO>> GetReceivedRatingsByUserByPostId(Guid postId);
        Task<decimal?> GetAverageRatingByUserId(Guid userId);
    }
}
