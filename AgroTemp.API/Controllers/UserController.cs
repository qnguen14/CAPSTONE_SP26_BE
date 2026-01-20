using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    
    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet(ApiEndpointConstants.User.GetAllUsersEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsers();

            var apiResponse = new ApiResponse<IEnumerable<UserDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Users retrieved successfully",
                Data = users
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    [HttpGet(ApiEndpointConstants.User.GetUserByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDTO>> GetUserById([FromRoute] Guid id)
    {
        try
        {
            var user = await _userService.GetUserById(id);

            var apiResponse = new ApiResponse<UserDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "User retrieved successfully",
                Data = user
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = ex.Message.Contains("not found") ? StatusCodes.Status404NotFound : StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(apiResponse.StatusCode, apiResponse);
        }
    }

    [HttpPost(ApiEndpointConstants.User.CreateUserEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDTO>> CreateUser([FromBody] CreateUserRequest request)
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

            var user = await _userService.CreateUser(request);

            var successResponse = new ApiResponse<UserDTO>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "User created successfully",
                Data = user
            };
            return StatusCode(StatusCodes.Status201Created, successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = ex.Message.Contains("already exists") ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(apiResponse.StatusCode, apiResponse);
        }
    }

    [HttpPut(ApiEndpointConstants.User.UpdateUserEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDTO>> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserRequest request)
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

            var user = await _userService.UpdateUser(id, request);

            var successResponse = new ApiResponse<UserDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "User updated successfully",
                Data = user
            };
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            
            int statusCode;
            if (ex.Message.Contains("not found"))
                statusCode = StatusCodes.Status404NotFound;
            else if (ex.Message.Contains("already exists"))
                statusCode = StatusCodes.Status400BadRequest;
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

    [HttpDelete(ApiEndpointConstants.User.DeleteUserEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteUser([FromRoute] Guid id)
    {
        try
        {
            var result = await _userService.DeleteUser(id);

            var apiResponse = new ApiResponse<bool>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "User deactivated successfully",
                Data = result
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = ex.Message.Contains("not found") ? StatusCodes.Status404NotFound : StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(apiResponse.StatusCode, apiResponse);
        }
    }
}