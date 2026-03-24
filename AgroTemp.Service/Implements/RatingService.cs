using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Rating;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                            .Include(r => r.Ratee)
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
                            .Include(r => r.Ratee)
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
                var rating = _mapper.CreateRatingRequestToRating(request);
                if (rating.Id == Guid.Empty)
                {
                    rating.Id = Guid.NewGuid();
                }

                await _unitOfWork.GetRepository<Rating>().InsertAsync(rating);
                await _unitOfWork.SaveChangesAsync();

                var createdRating = await _unitOfWork.GetRepository<Rating>()
                    .FirstOrDefaultAsync(
                        predicate: r => r.Id == rating.Id,
                        include: q => q
                            .Include(r => r.Rater)
                            .Include(r => r.Ratee)
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
                        predicate: r => r.Id == id);

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
    }
}
