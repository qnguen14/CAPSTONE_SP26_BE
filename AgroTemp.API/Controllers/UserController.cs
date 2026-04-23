using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Admin;
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
    [ProducesResponseType(typeof(AdminUserListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminUserListResponse>> GetAllUsers([FromQuery] AdminUserListQuery query)
    {
        try
        {
            var users = await _userService.GetAdminUsers(query);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet(ApiEndpointConstants.User.GetUserByIdEndpoint)]
    [ProducesResponseType(typeof(AdminUserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminUserDetailDto>> GetUserById([FromRoute] Guid id)
    {
        try
        {
            var user = await _userService.GetAdminUserById(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            var statusCode = ex.Message.Contains("not found") ? StatusCodes.Status404NotFound : StatusCodes.Status500InternalServerError;
            return StatusCode(statusCode, new { message = ex.Message });
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
    [ProducesResponseType(typeof(AdminUserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminUserDetailDto>> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data", errors = ModelState });
            }

            var user = await _userService.UpdateAdminUser(id, request);
            return Ok(user);
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

            return StatusCode(statusCode, new { message = ex.Message });
        }
    }

    [HttpDelete(ApiEndpointConstants.User.DeleteUserEndpoint)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteUser([FromRoute] Guid id)
    {
        try
        {
            var result = await _userService.DeleteAdminUser(id);
            return Ok(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            var statusCode = ex.Message.Contains("not found") ? StatusCodes.Status404NotFound : StatusCodes.Status500InternalServerError;
            return StatusCode(statusCode, new { message = ex.Message });
        }
    }
}
