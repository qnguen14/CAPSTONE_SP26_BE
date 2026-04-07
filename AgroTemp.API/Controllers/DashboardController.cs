using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.FarmerProfile;
using AgroTemp.Domain.DTO.WorkerProfile;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;
    
    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet(ApiEndpointConstants.Dashboard.FarmerDashboardEndpoint)]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<FarmerDashboardDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<FarmerDashboardDTO>>> GetFarmerDashboard()
    {
        try 
        {
            var result = await _dashboardService.GetFarmerDashboardAsync();
            return Ok(new ApiResponse<FarmerDashboardDTO> 
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farmer dashboard data retrieved successfully",
                Data = result   
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving farmer dashboard");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Dashboard.WorkerDashboardEndpoint)]
    [Authorize(Roles = "Worker")]
    [ProducesResponseType(typeof(ApiResponse<WorkerApplicationStatsDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<WorkerApplicationStatsDTO>>> GetWorkerDashboard()
    {
        try
        {
            var result = await _dashboardService.GetWorkerDashboardAsync();
            return Ok(new ApiResponse<WorkerApplicationStatsDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Worker dashboard data retrieved successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Worker profile not found when fetching dashboard");
            return NotFound(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving worker dashboard");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }
}