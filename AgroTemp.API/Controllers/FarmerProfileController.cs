using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize(Roles = "Farmer")]
public class FarmerProfileController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<FarmerProfileController> _logger;
    
    public FarmerProfileController(IUserService userService, ILogger<FarmerProfileController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get farmer profile by user ID - Only accessible by the farmer who owns the profile
    /// </summary>
    [HttpGet(ApiEndpointConstants.FarmerProfile.GetFarmerProfileEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<FarmerProfileDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FarmerProfileDTO>> GetFarmerProfile()
    {
        try
        {
            var profile = await _userService.GetFarmerProfile();

            var apiResponse = new ApiResponse<FarmerProfileDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farmer profile retrieved successfully",
                Data = profile
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving farmer profile");
            
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    [HttpPut(ApiEndpointConstants.FarmerProfile.UpdateFarmerProfileEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<FarmerProfileDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FarmerProfileDTO>> UpdateFarmerProfile(
        [FromBody] UpdateFarmerProfileRequest request)
    {
        try
        {
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

            var profile = await _userService.UpdateFarmerProfile(request);

            var successResponse = new ApiResponse<FarmerProfileDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farmer profile updated successfully",
                Data = profile
            };
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating farmer profile");
            
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    [HttpPost(ApiEndpointConstants.FarmerProfile.UploadAvatarEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UploadAvatar(IFormFile image)
    {
        try
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(ApiResponseBuilder.BuildResponse<object>(400, "Image file is required", null));
            }

            var imageUrl = await _userService.UploadFarmerAvatar(image);

            return Ok(ApiResponseBuilder.BuildResponse(200, "Avatar uploaded successfully", imageUrl));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading farmer avatar");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseBuilder.BuildResponse<object>(500, ex.Message, null));
        }
    }
}
