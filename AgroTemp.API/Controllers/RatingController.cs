using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Rating;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers
{
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly ILogger<RatingController> _logger;
        private readonly IRatingService _ratingService;

        public RatingController(ILogger<RatingController> logger, IRatingService ratingService)
        {
            _logger = logger;
            _ratingService = ratingService;
        }

        [HttpGet(ApiEndpointConstants.Rating.GetAllRatingsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RatingDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetAllRatings()
        {
            try
            {
                var response = await _ratingService.GetAllRatings();

                var apiResponse = new ApiResponse<IEnumerable<RatingDTO>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Ratings retrieved successfully",
                    Data = response
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving ratings",
                    Data = null
                });
            }
        }

        [HttpGet(ApiEndpointConstants.Rating.GetRatingByIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<RatingDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RatingDTO>> GetRatingById([FromRoute] Guid id)
        {
            try
            {
                var response = await _ratingService.GetRatingById(id);
                if (response == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Rating not found",
                        Data = null
                    });
                }
                var apiResponse = new ApiResponse<RatingDTO>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Rating retrieved successfully",
                    Data = response
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving the rating",
                    Data = null
                });
            }
        }

        [HttpPost(ApiEndpointConstants.Rating.CreateRatingEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<RatingDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RatingDTO>> CreateRating([FromBody] CreateRatingRequest request)
        {
            try
            {
                var response = await _ratingService.CreateRating(request);
                var apiResponse = new ApiResponse<RatingDTO>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Rating created successfully",
                    Data = response
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rating");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while creating the rating",
                    Data = null
                });
            }
        }

        [HttpPut(ApiEndpointConstants.Rating.UpdateRatingEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<RatingDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RatingDTO>> UpdateRating([FromRoute] Guid id, [FromBody] UpdateRatingRequest request)
        {
            try
            {
                var response = await _ratingService.UpdateRating(id, request);
                if (response == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Rating not found",
                        Data = null
                    });
                }
                var apiResponse = new ApiResponse<RatingDTO>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Rating updated successfully",
                    Data = response
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rating");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the rating",
                    Data = null
                });
            }
        }

        [HttpDelete(ApiEndpointConstants.Rating.DeleteRatingEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteRating([FromRoute] Guid id)
        {
            try
            {
                var result = await _ratingService.DeleteRating(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Rating not found",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Rating deleted successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rating");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while deleting the rating",
                    Data = null
                });
            }
        }

        [HttpGet(ApiEndpointConstants.Rating.GetSpecificRatingByUserIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<RatingDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RatingDTO>> GetSpecificRatingByUserId([FromRoute] Guid id)
        {
            try
            {
                var response = await _ratingService.GetSpecificRatingByUserId(id);
                if (response == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Rating not found for the user",
                        Data = null
                    });
                }
                var apiResponse = new ApiResponse<RatingDTO>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Rating retrieved successfully for the user",
                    Data = response
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating for the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving the rating for the user",
                    Data = null
                });
            }
        }

        [HttpGet(ApiEndpointConstants.Rating.GetAllRatingsByUserIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RatingDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetAllRatingsByUserId([FromRoute] Guid userId)
        {
            try
            {
                var response = await _ratingService.GetAllRatingsByUserId(userId);
                if (response == null || !response.Any())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No ratings found for the user",
                        Data = null
                    });
                }
                var apiResponse = new ApiResponse<IEnumerable<RatingDTO>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Ratings retrieved successfully for the user",
                    Data = response
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving ratings for the user",
                    Data = null
                });
            }
        }

        [HttpGet(ApiEndpointConstants.Rating.GetAverageRatingByUserIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<double>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<decimal?>> GetAverageRatingByUserId([FromRoute] Guid userId)
        {
            try
            {
                var response = await _ratingService.GetAverageRatingByUserId(userId);
                if (response == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No ratings found for the user",
                        Data = null
                    });
                }
                var apiResponse = new ApiResponse<decimal?>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Average rating retrieved successfully for the user",
                    Data = response
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving average rating for the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving the average rating for the user",
                    Data = null
                });
            }
        }
    }
}
