using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Rating;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Service.Implements
{
    public class RatingService : BaseService<Rating>, IRatingService
    {
        public RatingService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<RatingDTO>> GetAllRatings()
        {
            try
            {
                var ratings = await _unitOfWork.GetRepository<Rating>()
                    .GetListAsync(
                        predicate: null,
                        include: q => q
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.JobPost),
                        orderBy: r => r.OrderBy(x => x.CreatedAt));
                if (ratings == null || !ratings.Any())
                {
                    return null;
                }
                var result = _mapper.RatingsToRatingDtos(ratings);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RatingDTO> GetRatingById(Guid id)
        {
            try
            {
                var rating = await _unitOfWork.GetRepository<Rating>()
                    .FirstOrDefaultAsync(
                        predicate: r => r.Id == id,
                        include: q => q
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.JobPost));
                if (rating == null)
                {
                    return null;
                }
                var result = _mapper.RatingToRatingDto(rating);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RatingDTO> CreateRating(CreateRatingRequest request)
        {
            try
            {
                var raterUserId = GetCurrentUserId();

                var ratee = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(predicate: u => u.Id == request.RateeId);
                if (ratee == null)
                    throw new KeyNotFoundException($"Ratee with ID {request.RateeId} does not exist. Make sure you are passing the User ID, not the Worker/Farmer profile ID.");

                var jobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(predicate: j => j.Id == request.JobPostId);
                if (jobPost == null)
                    throw new KeyNotFoundException($"Job post with ID {request.JobPostId} does not exist.");

                var existingRating = await _unitOfWork.GetRepository<Rating>()
                    .FirstOrDefaultAsync(predicate: r =>
                        r.RaterId == raterUserId &&
                        r.RateeId == request.RateeId &&
                        r.JobPostId == request.JobPostId);

                if (existingRating != null)
                    throw new InvalidOperationException("You have already submitted a rating for this user on this job post.");

                var rating = _mapper.CreateRatingRequestToRating(request);
                if (rating.Id == Guid.Empty)
                {
                    rating.Id = Guid.NewGuid();
                    rating.RaterId = raterUserId;
                }

                await _unitOfWork.GetRepository<Rating>().InsertAsync(rating);
                await _unitOfWork.SaveChangesAsync();

                var allRatingsForRatee = await _unitOfWork.GetRepository<Rating>()
                    .GetListAsync(predicate: r => r.RateeId == request.RateeId);

                var newAverage = (decimal)allRatingsForRatee.Average(r => r.RatingScore);

                if (request.TypeId == (int)RatingType.FarmerToWorker)
                {
                    var worker = await _unitOfWork.GetRepository<Worker>()
                        .FirstOrDefaultAsync(predicate: w => w.UserId == request.RateeId);

                    if (worker != null)
                    {
                        worker.AverageRating = newAverage;
                        _unitOfWork.GetRepository<Worker>().UpdateAsync(worker);
                    }
                }
                else if (request.TypeId == (int)RatingType.WorkerToFarmer)
                {
                    var farmer = await _unitOfWork.GetRepository<Farmer>()
                        .FirstOrDefaultAsync(predicate: f => f.UserId == request.RateeId);

                    if (farmer != null)
                    {
                        farmer.AverageRating = newAverage;
                        _unitOfWork.GetRepository<Farmer>().UpdateAsync(farmer);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                var createdRating = await _unitOfWork.GetRepository<Rating>()
                    .FirstOrDefaultAsync(
                        predicate: r => r.Id == rating.Id,
                        include: q => q
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.JobPost));

                var result = _mapper.RatingToRatingDto(createdRating ?? rating);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RatingDTO> UpdateRating(Guid id, UpdateRatingRequest request)
        {
            try
            {
                var existingRating = await _unitOfWork.GetRepository<Rating>()
                    .FirstOrDefaultAsync(
                        predicate: r => r.Id == id,
                        include: q => q
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.JobPost));

                if (existingRating == null)
                {
                    return null;
                }

                _mapper.UpdateRatingRequestToRating(request, existingRating);
                _unitOfWork.GetRepository<Rating>().UpdateAsync(existingRating);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.RatingToRatingDto(existingRating);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteRating(Guid id)
        {
            try
            {
                var existingRating = await _unitOfWork.GetRepository<Rating>()
                    .FirstOrDefaultAsync(
                        predicate: r => r.Id == id);
                if (existingRating == null)
                {
                    return false;
                }
                _unitOfWork.GetRepository<Rating>().DeleteAsync(existingRating);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RatingDTO> GetSpecificRatingByUserId(Guid userId)
        {
            try
            {
                var rating = await _unitOfWork.GetRepository<Rating>()
                    .FirstOrDefaultAsync(
                        predicate: r => r.RateeId == userId,
                        include: q => q
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.JobPost));
                if (rating == null)
                {
                    return null;
                }
                var result = _mapper.RatingToRatingDto(rating);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<RatingDTO>> GetAllRatingsByUserId(Guid userId)
        {
            try
            {
                var ratings = await _unitOfWork.GetRepository<Rating>()
                    .GetListAsync(
                        predicate: r => r.RateeId == userId,
                        include: q => q
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.JobPost));
                if (ratings == null || !ratings.Any())
                {
                    return null;
                }
                var result = _mapper.RatingsToRatingDtos(ratings);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<RatingDTO>> GetGivenRatingsByUser()
        {
            try
            {
                var userId = GetCurrentUserId();

                var ratings = await _unitOfWork.GetRepository<Rating>()
                    .GetListAsync(
                        predicate: r => r.RaterId == userId,
                        include: q => q
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.JobPost));

                if (ratings == null || !ratings.Any())
                    throw new KeyNotFoundException("No ratings found for the current user.");

                var result = _mapper.RatingsToRatingDtos(ratings);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<RatingDTO>> GetReceivedRatingsByUserByPostId(Guid postId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var ratings = await _unitOfWork.GetRepository<Rating>()
                    .GetListAsync(
                        predicate: r => r.RateeId == userId && r.JobPostId == postId,
                        include: q => q
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Rater)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Worker)
                            .Include(r => r.Ratee)
                                .ThenInclude(u => u.Farmer)
                            .Include(r => r.JobPost));

                if (ratings == null || !ratings.Any())
                    throw new KeyNotFoundException("No ratings found for the current user on this post.");

                var result = _mapper.RatingsToRatingDtos(ratings);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<decimal?> GetAverageRatingByUserId(Guid userId)
        {
            try
            {
                var ratings = await _unitOfWork.GetRepository<Rating>()
                    .GetListAsync(
                        predicate: r => r.RateeId == userId,
                        include: q => q
                            .Include(r => r.Rater)
                            .Include(r => r.Ratee)
                            .Include(r => r.JobPost));

                if (ratings == null || !ratings.Any())
                {
                    return null;
                }

                var averageScore = ratings.Average(r => r.RatingScore);
                
                return (decimal)averageScore;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
