using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Admin;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Implements;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminDashboardService _adminDashboardService;
    private readonly ILogger<AdminController> _logger;
    private readonly IJobPostService _jobPostService;
    private readonly IPayOSService _payOSService;
    private readonly IWalletService _walletService;
    private readonly IWalletTransactionService _walletTransactionService;

    public AdminController(
        IAdminDashboardService adminDashboardService,
        ILogger<AdminController> logger,
        IJobPostService jobPostService,
        IPayOSService payOSService,
        IWalletService walletService,
        IWalletTransactionService walletTransactionService)
    {
        _adminDashboardService = adminDashboardService;
        _logger = logger;
        _jobPostService = jobPostService;
        _payOSService = payOSService;
        _walletService = walletService;
        _walletTransactionService = walletTransactionService;
    }

    [HttpGet(ApiEndpointConstants.Admin.GetDashboardEndpoint)]
    [ProducesResponseType(typeof(AdminDashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetDashboard()
    {
        try
        {
            var dashboard = await _adminDashboardService.GetDashboardAsync();
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve admin dashboard");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to retrieve admin dashboard", detail = ex.Message });
        }
    }

    [HttpGet(ApiEndpointConstants.Admin.GetJobpostEndpoint)]
    [ProducesResponseType(typeof(PaginatedAdminJobPostsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedAdminJobPostsResponse>> GetJobPostsForAdmin([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        try
        {
            var response = await _jobPostService.GetJobPostsForAdmin(page, limit);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to admin job posts");
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin job posts");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving admin job posts" });
        }
    }

    [HttpGet(ApiEndpointConstants.Admin.WalletStatsEndpoint)]
    [ProducesResponseType(typeof(AgroTemp.Domain.DTO.Payment.AdminWalletStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetWalletStats([FromQuery] DateTime? date = null)
    {
        try
        {
            var stats = await _payOSService.GetAdminWalletStatsAsync(date);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve admin wallet stats");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to retrieve admin wallet stats", detail = ex.Message });
        }
    }

    [HttpGet(ApiEndpointConstants.Admin.WithdrawalsEndpoint)]
    [ProducesResponseType(typeof(AgroTemp.Domain.DTO.Payment.PaginatedAdminWithdrawalsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetWithdrawalsForAdmin([FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? status = null, [FromQuery] string? search = null)
    {
        try
        {
            var response = await _payOSService.GetWithdrawalsForAdminAsync(page, limit, status, search);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve withdrawals for admin");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to retrieve withdrawals", detail = ex.Message });
        }
    }

    [HttpGet(ApiEndpointConstants.Admin.WalletsEndpoint)]
    [ProducesResponseType(typeof(AgroTemp.Domain.DTO.Payment.PaginatedAdminWalletsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetWalletsForAdmin([FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? search = null)
    {
        try
        {
            var response = await _walletService.GetWalletsForAdminAsync(page, limit, search);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve wallets for admin");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to retrieve wallets", detail = ex.Message });
        }
    }

    [HttpGet(ApiEndpointConstants.Admin.WalletTransactionsEndpoint)]
    [ProducesResponseType(typeof(AgroTemp.Domain.DTO.Payment.PaginatedAdminWalletTransactionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetWalletTransactionsForAdmin([FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] Domain.Entities.TransactionType? type = null, [FromQuery] string? search = null)
    {
        try
        {
            var response = await _walletTransactionService.GetWalletTransactionsForAdminAsync(page, limit, type, null, search);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve wallet transactions for admin");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to retrieve wallet transactions", detail = ex.Message });
        }
    }
}
