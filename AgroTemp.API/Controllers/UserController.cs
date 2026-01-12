using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
// [Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    
    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    /// <summary>
    /// Lấy tất cả người dùng
    /// </summary>
    [HttpGet(ApiEndpointConstants.User.GetAllUsersEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<User>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        try
        {
            // Service returns all users with basic profile information
            // Includes role information and account status
            var response = await _userService.GetAllUsers();

            var apiResponse = new ApiResponse<IEnumerable<UserDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Users retrieved successfully",
                Data = response // Complete user list for administrative purposes
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
    
}