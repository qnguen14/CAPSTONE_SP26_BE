using AgroTemp.API.Constants;
using AgroTemp.API.Models.Farm;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Farm;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Implements;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize(Roles = "Farmer")]
public class FarmController : Controller
{
    private readonly IFarmService _farmService;
    private readonly IUserService _userService;
    private readonly ILogger<FarmController> _logger;

    public FarmController(IFarmService farmService, IUserService userService, ILogger<FarmController> logger)
    {
        _farmService = farmService;
        _userService = userService;
        _logger = logger;
    }

    [HttpGet(ApiEndpointConstants.Farm.GetFarmsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FarmDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<FarmDTO>>> GetMyFarms()
    {
        try
        {
            var farmerProfile = await _userService.GetFarmerProfile();

            var farms = await _farmService.GetFarmByFarmer(farmerProfile.Id);

            var apiResponse = new ApiResponse<IEnumerable<FarmDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farms retrieved successfully",
                Data = farms
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving farms");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    [HttpGet(ApiEndpointConstants.Farm.GetFarmByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<FarmDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FarmDTO>> GetFarmById([FromRoute] Guid id)
    {
        try
        {
            var farm = await _farmService.GetFarmById(id);

            var apiResponse = new ApiResponse<FarmDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farm retrieved successfully",
                Data = farm
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving farm {FarmId}", id);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = ex.Message.Contains("not found") ? StatusCodes.Status404NotFound : StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(apiResponse.StatusCode, apiResponse);
        }
    }

    [HttpPost(ApiEndpointConstants.Farm.CreateFarmEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<FarmDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FarmDTO>> CreateFarm([FromBody] CreateFarmRequest request)
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

            var farmerProfile = await _userService.GetFarmerProfile();

            var farm = await _farmService.CreateFarm(farmerProfile.Id, request);

            var successResponse = new ApiResponse<FarmDTO>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Farm created successfully",
                Data = farm
            };
            return StatusCode(StatusCodes.Status201Created, successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating farm");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    [HttpPut(ApiEndpointConstants.Farm.UpdateFarmEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<FarmDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FarmDTO>> UpdateFarm([FromRoute] Guid id, [FromBody] UpdateFarmRequest request)
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

            var farmerProfile = await _userService.GetFarmerProfile();

            var farm = await _farmService.UpdateFarm(id, farmerProfile.Id, request);

            var successResponse = new ApiResponse<FarmDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farm updated successfully",
                Data = farm
            };
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating farm {FarmId}", id);

            int statusCode;
            if (ex.Message.Contains("not found"))
                statusCode = StatusCodes.Status404NotFound;
            else if (ex.Message.Contains("only update your own"))
                statusCode = StatusCodes.Status403Forbidden;
            else
                statusCode = StatusCodes.Status500InternalServerError;

            var apiResponse = new ApiResponse<object>
            {
                StatusCode = statusCode,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(statusCode, apiResponse);
        }
    }

    [HttpDelete(ApiEndpointConstants.Farm.DeleteFarmEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteFarm([FromRoute] Guid id)
    {
        try
        {
            var farmerProfile = await _userService.GetFarmerProfile();

            var result = await _farmService.DeleteFarm(id, farmerProfile.Id);

            var apiResponse = new ApiResponse<bool>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farm deleted successfully",
                Data = result
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting farm {FarmId}", id);

            int statusCode;
            if (ex.Message.Contains("not found"))
                statusCode = StatusCodes.Status404NotFound;
            else if (ex.Message.Contains("only delete your own"))
                statusCode = StatusCodes.Status403Forbidden;
            else
                statusCode = StatusCodes.Status500InternalServerError;

            var apiResponse = new ApiResponse<object>
            {
                StatusCode = statusCode,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(statusCode, apiResponse);
        }
    }

    [HttpPost(ApiEndpointConstants.Farm.UploadFarmImageEndpoint)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UploadFarmImage([FromRoute] Guid id, [FromForm] UploadFarmImageRequest request)
    {
        try
        {
            if (request?.File == null || request.File.Length == 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Image file is required",
                    Data = null
                });
            }

            var farmerProfile = await _userService.GetFarmerProfile();
            var imageUrl = await _farmService.UploadFarmImage(id, farmerProfile.Id, request.File);

            return Ok(new ApiResponse<string>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farm image uploaded successfully",
                Data = imageUrl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading farm image for farm {FarmId}", id);

            int statusCode;
            if (ex.Message.Contains("not found"))
                statusCode = StatusCodes.Status404NotFound;
            else if (ex.Message.Contains("only update your own"))
                statusCode = StatusCodes.Status403Forbidden;
            else if (ex.Message.Contains("required"))
                statusCode = StatusCodes.Status400BadRequest;
            else
                statusCode = StatusCodes.Status500InternalServerError;

            return StatusCode(statusCode, new ApiResponse<object>
            {
                StatusCode = statusCode,
                Message = ex.Message,
                Data = null
            });
        }
    }
}