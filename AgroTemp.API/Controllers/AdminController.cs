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

    public AdminController(
        IAdminDashboardService adminDashboardService,
        ILogger<AdminController> logger, IJobPostService jobPostService)
    {
        _adminDashboardService = adminDashboardService;
        _logger = logger;
        _jobPostService = jobPostService;
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
}
