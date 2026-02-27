using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Auth;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Sign in 
    /// </summary>
    [HttpPost(ApiEndpointConstants.Auth.LoginEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.Login(request);

            var apiResponse = new ApiResponse<LoginResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Login successful",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            };
            return Unauthorized(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred during login",
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    /// <summary>
    /// Register
    /// </summary>
    [HttpPost(ApiEndpointConstants.Auth.RegisterEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.Register(request);

            var apiResponse = new ApiResponse<LoginResponse>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Registration successful",
                Data = response
            };
            return StatusCode(StatusCodes.Status201Created, apiResponse);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            };
            return BadRequest(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred during registration",
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    /// <summary>
    /// Sign-in with Google 
    /// </summary>
    [HttpPost(ApiEndpointConstants.Auth.GoogleLoginEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var response = await _authService.GoogleLogin(request);

            var apiResponse = new ApiResponse<LoginResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Google login successful",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            };
            return Unauthorized(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google login");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred during Google login",
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    /// <summary>
    /// Logout
    /// </summary>
    
    [Authorize]
    [HttpPost(ApiEndpointConstants.Auth.LogoutEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Logout()
    {
        try{
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader!["Bearer".Length..].Trim();
            await _authService.Logout(token);

            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Logout successful",
                Data = null
            };
            return Ok(apiResponse);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            };
            return BadRequest(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred during logout",
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }
}
