using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Payment;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
public class WithdrawController : ControllerBase
{
    private readonly IPayOSService _payOSService;
    private readonly ILogger<WithdrawController> _logger;

    public WithdrawController(IPayOSService payOSService, ILogger<WithdrawController> logger)
    {
        _payOSService = payOSService;
        _logger = logger;
    }

    [HttpPost(ApiEndpointConstants.Withdraw.WithdrawEndpoint)]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<WithdrawalResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WithdrawalResponse>> CreateWithdrawal([FromBody] CreateWithdrawalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid withdrawal request",
                Data = ModelState
            });
        }

        try
        {
            var withdrawal = await _payOSService.CreateWithdrawalAsync(request);
            return StatusCode(StatusCodes.Status201Created, new ApiResponse<WithdrawalResponse>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Withdrawal request created successfully",
                Data = withdrawal
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
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
            _logger.LogError(ex, "Failed to create withdrawal request");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to create withdrawal request",
                Data = ex.Message
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Withdraw.WithdrawByIdEndpoint)]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<WithdrawalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WithdrawalResponse>> GetWithdrawal([FromRoute] Guid id)
    {
        try
        {
            var withdrawal = await _payOSService.GetWithdrawalAsync(id);
            if (withdrawal == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Withdrawal request not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<WithdrawalResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Withdrawal request retrieved successfully",
                Data = withdrawal
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve withdrawal request {WithdrawalId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve withdrawal request",
                Data = ex.Message
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Withdraw.WithdrawEndpoint)]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ICollection<WithdrawalResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ICollection<WithdrawalResponse>>> GetMyWithdrawals()
    {
        try
        {
            var withdrawals = await _payOSService.GetMyWithdrawalsAsync();
            return Ok(new ApiResponse<ICollection<WithdrawalResponse>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Withdrawal requests retrieved successfully",
                Data = withdrawals
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve withdrawal requests");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve withdrawal requests",
                Data = ex.Message
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Withdraw.WithdrawBalanceEndpoint)]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<WithdrawalAccountBalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WithdrawalAccountBalanceResponse>> GetWithdrawAccountBalance()
    {
        try
        {
            var balance = await _payOSService.GetWithdrawalAccountBalanceAsync();
            return Ok(new ApiResponse<WithdrawalAccountBalanceResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Withdraw account balance retrieved successfully",
                Data = balance
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve withdraw account balance");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve withdraw account balance",
                Data = ex.Message
            });
        }
    }
}
