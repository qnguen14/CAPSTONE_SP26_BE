using AgroTemp.API.Constants;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize]
public class WalletTransactionController : ControllerBase
{
    private readonly IWalletTransactionService _walletTransactionService;
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletTransactionController> _logger;

    public WalletTransactionController(
        IWalletTransactionService walletTransactionService,
        IWalletService walletService,
        ILogger<WalletTransactionController> logger)
    {
        _walletTransactionService = walletTransactionService;
        _walletService = walletService;
        _logger = logger;
    }

    [HttpGet(ApiEndpointConstants.WalletTransaction.GetAllWalletTransactionsEndpoint)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllWalletTransactions()
    {
        try
        {
            var transactions = await _walletTransactionService.GetAllAsync();
            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Wallet transactions retrieved successfully",
                Data = transactions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve wallet transactions");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve wallet transactions",
                Data = ex.Message
            });
        }
    }

    [HttpGet(ApiEndpointConstants.WalletTransaction.GetWalletTransactionByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetWalletTransactionById([FromRoute] Guid id)
    {
        try
        {
            var transaction = await _walletTransactionService.GetByIdAsync(id);
            if (transaction == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Wallet transaction not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Wallet transaction retrieved successfully",
                Data = transaction
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve wallet transaction {TransactionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve wallet transaction",
                Data = ex.Message
            });
        }
    }

    [HttpGet(ApiEndpointConstants.WalletTransaction.GetWalletTransactionsByWalletIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetWalletTransactionsByWalletId([FromRoute] Guid walletId)
    {
        try
        {
            // Verify ownership of wallet
            var wallet = await _walletService.GetByIdAsync(walletId);
            if (wallet == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Wallet not found",
                    Data = null
                });
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId) && wallet.UserId != userId)
            {
                var isAdmin = User.IsInRole("Admin");
                if (!isAdmin)
                {
                    return Forbid();
                }
            }

            var transactions = await _walletTransactionService.GetByWalletIdAsync(walletId);
            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Wallet transactions retrieved successfully",
                Data = transactions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve wallet transactions for wallet {WalletId}", walletId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve wallet transactions",
                Data = ex.Message
            });
        }
    }
}
