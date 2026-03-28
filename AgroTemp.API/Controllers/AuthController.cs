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
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
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
        catch (InvalidOperationException ex) when (ex.Message == "EMAIL_NOT_VERIFIED")
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = "Email not verified. Please check your inbox for the verification code.",
                Data = null
            });
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
    /// Register — sends a verification OTP to the provided email
    /// </summary>
    [HttpPost(ApiEndpointConstants.Auth.RegisterEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _authService.Register(request);

            return StatusCode(StatusCodes.Status201Created, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Registration successful. Please check your email for the verification code.",
                Data = null
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred during registration",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Auth.VerifyEmailEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        try
        {
            var response = await _authService.VerifyEmail(request);
            return Ok(new ApiResponse<LoginResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Email verified successfully.",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email verification");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred during email verification",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Auth.ResendVerificationEndpoint)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResendVerification([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            await _authService.ResendVerificationEmail(request.Email);
            return Ok(new { Message = "If the email is registered and unverified, a new code has been sent." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during resend verification");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while resending the verification email",
                Data = null
            });
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
    /// Forgot Password - Request password reset
    /// </summary>
    [HttpPost(ApiEndpointConstants.Auth.ForgetPasswordEndpoint)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            await _authService.ForgotPassword(request);
            return Ok(new { Message = "If the email exists, a password reset code has been sent" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while processing your request",
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    /// <summary>
    /// Reset Password - Reset password with OTP
    /// </summary>
    [HttpPost(ApiEndpointConstants.Auth.ResetPasswordEndpoint)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPassword(request);
        if (result)
        {
            return Ok(new { Message = "Password reset successfully" });
        }
        return BadRequest(new { Message = "Invalid email or OTP, or OTP has expired" });
    }

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
