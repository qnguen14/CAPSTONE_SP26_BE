using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize(Roles = "Worker")]
public class WorkerProfileController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<WorkerProfileController> _logger;
    
    public WorkerProfileController(IUserService userService, ILogger<WorkerProfileController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get worker profile by user ID - Only accessible by the worker who owns the profile
    /// </summary>
    [HttpGet(ApiEndpointConstants.WorkerProfile.GetWorkerProfileEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<WorkerProfileDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WorkerProfileDTO>> GetWorkerProfile([FromRoute] Guid userId)
    {
        try
        {
            // Verify the authenticated user matches the userId in the route
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (currentUserId == null || currentUserId != userId.ToString())
            {
                var forbiddenResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "You can only access your own profile",
                    Data = null
                };
                return StatusCode(StatusCodes.Status403Forbidden, forbiddenResponse);
            }

            var profile = await _userService.GetWorkerProfile(userId);

            var apiResponse = new ApiResponse<WorkerProfileDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Worker profile retrieved successfully",
                Data = profile
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving worker profile for user {UserId}", userId);
            
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    /// <summary>
    /// Update worker profile - Creates profile on first update if it doesn't exist
    /// Only the worker who owns the profile can update it
    /// </summary>
    [HttpPut(ApiEndpointConstants.WorkerProfile.UpdateWorkerProfileEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<WorkerProfileDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WorkerProfileDTO>> UpdateWorkerProfile(
        [FromRoute] Guid userId, 
        [FromBody] UpdateWorkerProfileRequest request)
    {
        try
        {
            // Verify the authenticated user matches the userId in the route
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (currentUserId == null || currentUserId != userId.ToString())
            {
                var forbiddenResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "You can only update your own profile",
                    Data = null
                };
                return StatusCode(StatusCodes.Status403Forbidden, forbiddenResponse);
            }

            if (!ModelState.IsValid)
            {
                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid request data",
                    Data = ModelState
                };
                return BadRequest(apiResponse);
            }

            var profile = await _userService.UpdateWorkerProfile(userId, request);

            var successResponse = new ApiResponse<WorkerProfileDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Worker profile updated successfully",
                Data = profile
            };
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating worker profile for user {UserId}", userId);
            
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }
}
