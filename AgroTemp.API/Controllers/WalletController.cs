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

    [HttpGet(ApiEndpointConstants.Wallet.GetAllWalletsEndpoint)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllWallets()
    {
        try
        {
            var wallets = await _walletService.GetAllAsync();
            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Wallets retrieved successfully",
                Data = wallets
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve wallets");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve wallets",
                Data = ex.Message
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Wallet.GetWalletByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
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
