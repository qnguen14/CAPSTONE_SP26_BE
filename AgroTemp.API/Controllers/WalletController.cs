using AgroTemp.API.Constants;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletController> _logger;

    public WalletController(IWalletService walletService, ILogger<WalletController> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    // Admin wallet listing moved to AdminController

    [HttpGet(ApiEndpointConstants.Wallet.GetWalletByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin wallet by id.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get wallet by id.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WalletGetWalletById")]
    public async Task<ActionResult> GetWalletById([FromRoute] Guid id)
    {
        try
        {
            var wallet = await _walletService.GetByIdAsync(id);
            if (wallet == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Wallet not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Wallet retrieved successfully",
                Data = wallet
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve wallet {WalletId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve wallet",
                Data = ex.Message
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Wallet.GetMyWalletEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin wallet cua nguoi dung hien tai.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get my wallet.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WalletGetMyWallet")]
    public async Task<ActionResult> GetMyWallet()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "User not authenticated",
                    Data = null
                });
            }

            var wallet = await _walletService.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Your wallet not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Wallet retrieved successfully",
                Data = wallet
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve my wallet");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve wallet",
                Data = ex.Message
            });
        }
    }
}
