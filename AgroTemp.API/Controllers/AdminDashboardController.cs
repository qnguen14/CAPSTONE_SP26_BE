using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Admin;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _adminDashboardService;
    private readonly ILogger<AdminDashboardController> _logger;

    public AdminDashboardController(
        IAdminDashboardService adminDashboardService,
        ILogger<AdminDashboardController> logger)
    {
        _adminDashboardService = adminDashboardService;
        _logger = logger;
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
}
