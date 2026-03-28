using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.WorkerAttendance;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgroTemp.API.Controllers;

[ApiController]
public class WorkerAttendanceController : ControllerBase
{
    private readonly ILogger<WorkerAttendanceController> _logger;
    private readonly IWorkerAttendanceService _workerAttendanceService;

    public WorkerAttendanceController(
        ILogger<WorkerAttendanceController> logger,
        IWorkerAttendanceService workerAttendanceService)
    {
        _logger = logger;
        _workerAttendanceService = workerAttendanceService;
    }


    [HttpPost(ApiEndpointConstants.WorkerAttendance.CheckInEndpoint)]
    [Authorize(Roles = "Worker")]
    [ProducesResponseType(typeof(ApiResponse<WorkerAttendanceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Ghi nhan check in.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang check in.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WorkerAttendanceCheckIn")]
    public async Task<ActionResult<WorkerAttendanceDTO>> CheckIn([FromBody] CheckInRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid request data",
                    Data = ModelState
                };
                return BadRequest(apiResponse);
            }

            // Get worker profile ID from authenticated user
            var workerProfileIdClaim = User.FindFirst("WorkerProfileId")?.Value;
            
            if (string.IsNullOrEmpty(workerProfileIdClaim) || !Guid.TryParse(workerProfileIdClaim, out var workerProfileId))
            {
                var forbiddenResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Worker profile not found",
                    Data = null
                };
                return StatusCode(StatusCodes.Status403Forbidden, forbiddenResponse);
            }

            var response = await _workerAttendanceService.CheckIn(workerProfileId, request);

            var successResponse = new ApiResponse<WorkerAttendanceDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Checked in successfully",
                Data = response
            };
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during check-in for job application {JobApplicationId}", request.JobApplicationId);
            
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }


    [HttpPut(ApiEndpointConstants.WorkerAttendance.CheckOutEndpoint)]
    [Authorize(Roles = "Worker")]
    [ProducesResponseType(typeof(ApiResponse<WorkerAttendanceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Ghi nhan check out.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang check out.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WorkerAttendanceCheckOut")]
    public async Task<ActionResult<WorkerAttendanceDTO>> CheckOut([FromBody] CheckOutRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid request data",
                    Data = ModelState
                };
                return BadRequest(apiResponse);
            }

            // Get worker profile ID from authenticated user
            var workerProfileIdClaim = User.FindFirst("WorkerProfileId")?.Value;
            
            if (string.IsNullOrEmpty(workerProfileIdClaim) || !Guid.TryParse(workerProfileIdClaim, out var workerProfileId))
            {
                var forbiddenResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Worker profile not found",
                    Data = null
                };
                return StatusCode(StatusCodes.Status403Forbidden, forbiddenResponse);
            }

            var response = await _workerAttendanceService.CheckOut(workerProfileId, request);

            var successResponse = new ApiResponse<WorkerAttendanceDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Checked out successfully",
                Data = response
            };
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during check-out for attendance {AttendanceId}", request.AttendanceId);
            
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }


    [HttpGet(ApiEndpointConstants.WorkerAttendance.GetAttendanceByIdEndpoint)]
    [Authorize(Roles = "Worker,Farmer")]
    [ProducesResponseType(typeof(ApiResponse<WorkerAttendanceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin attendance by id.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get attendance by id.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WorkerAttendanceGetAttendanceById")]
    public async Task<ActionResult<WorkerAttendanceDTO>> GetAttendanceById([FromRoute] Guid id)
    {
        try
        {
            var response = await _workerAttendanceService.GetAttendanceById(id);

            var apiResponse = new ApiResponse<WorkerAttendanceDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Attendance record retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attendance record {AttendanceId}", id);
            
            if (ex.Message.Contains("not found"))
            {
                var notFoundResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Data = null
                };
                return NotFound(notFoundResponse);
            }

            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }


    [HttpGet(ApiEndpointConstants.WorkerAttendance.GetWorkerAttendanceHistoryEndpoint)]
    [Authorize(Roles = "Worker")]
    [ProducesResponseType(typeof(ApiResponse<List<WorkerAttendanceDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin worker attendance history.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get worker attendance history.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WorkerAttendanceGetWorkerAttendanceHistory")]
    public async Task<ActionResult<List<WorkerAttendanceDTO>>> GetWorkerAttendanceHistory(
        [FromRoute] Guid workerProfileId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            // Verify the authenticated user matches the workerProfileId in the route
            var workerProfileIdClaim = User.FindFirst("WorkerProfileId")?.Value;
            
            if (string.IsNullOrEmpty(workerProfileIdClaim) || workerProfileIdClaim != workerProfileId.ToString())
            {
                var forbiddenResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "You can only view your own attendance history",
                    Data = null
                };
                return StatusCode(StatusCodes.Status403Forbidden, forbiddenResponse);
            }

            var response = await _workerAttendanceService.GetWorkerAttendanceHistory(
                workerProfileId, 
                startDate, 
                endDate);

            var apiResponse = new ApiResponse<List<WorkerAttendanceDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Worker attendance history retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attendance history for worker {WorkerProfileId}", workerProfileId);
            
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    [HttpPut(ApiEndpointConstants.WorkerAttendance.ApproveAttendanceEndpoint)]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<WorkerAttendanceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Phe duyet attendance.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang approve attendance.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WorkerAttendanceApproveAttendance")]
    public async Task<ActionResult<WorkerAttendanceDTO>> ApproveAttendance([FromBody] ApproveAttendanceRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid request data",
                    Data = ModelState
                };
                return BadRequest(apiResponse);
            }

            // Get farmer profile ID from authenticated user
            var farmerProfileIdClaim = User.FindFirst("FarmerProfileId")?.Value;
            
            if (string.IsNullOrEmpty(farmerProfileIdClaim) || !Guid.TryParse(farmerProfileIdClaim, out var farmerProfileId))
            {
                var forbiddenResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Farmer profile not found",
                    Data = null
                };
                return StatusCode(StatusCodes.Status403Forbidden, forbiddenResponse);
            }

            var response = await _workerAttendanceService.ApproveAttendance(farmerProfileId, request);

            var successResponse = new ApiResponse<WorkerAttendanceDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Attendance approved successfully",
                Data = response
            };
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving attendance {AttendanceId}", request.AttendanceId);
            
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    [HttpGet(ApiEndpointConstants.WorkerAttendance.GetFarmAttendanceRecordsEndpoint)]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<List<WorkerAttendanceDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin farm attendance records.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get farm attendance records.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WorkerAttendanceGetFarmAttendanceRecords")]
    public async Task<ActionResult<List<WorkerAttendanceDTO>>> GetFarmAttendanceRecords(
        [FromRoute] Guid farmerProfileId,
        [FromQuery] Guid? jobPostId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            // Verify the authenticated user matches the farmerProfileId in the route
            var farmerProfileIdClaim = User.FindFirst("FarmerProfileId")?.Value;
            
            if (string.IsNullOrEmpty(farmerProfileIdClaim) || farmerProfileIdClaim != farmerProfileId.ToString())
            {
                var forbiddenResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "You can only view attendance records for your own farm",
                    Data = null
                };
                return StatusCode(StatusCodes.Status403Forbidden, forbiddenResponse);
            }

            var response = await _workerAttendanceService.GetFarmAttendanceRecords(
                farmerProfileId, 
                jobPostId, 
                startDate, 
                endDate);

            var apiResponse = new ApiResponse<List<WorkerAttendanceDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farm attendance records retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving farm attendance records for farmer {FarmerProfileId}", farmerProfileId);
            
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }

    //View Check-in/out for farmer 
    [HttpGet(ApiEndpointConstants.WorkerAttendance.GetWorkerAttendanceByFarmerEndpoint)]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<List<WorkerAttendanceDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin worker attendance by farmer.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get worker attendance by farmer.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WorkerAttendanceGetWorkerAttendanceByFarmer")]
    public async Task<ActionResult<List<WorkerAttendanceDTO>>> GetWorkerAttendanceByFarmer(
    [FromRoute] Guid farmerProfileId,
    [FromRoute] Guid workerProfileId,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
    {
        try
        {
            // Verify the authenticated user matches the farmerProfileId in the route
            var farmerProfileIdClaim = User.FindFirst("FarmerProfileId")?.Value;

            if (string.IsNullOrEmpty(farmerProfileIdClaim) || farmerProfileIdClaim != farmerProfileId.ToString())
            {
                var forbiddenResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "You can only view attendance for your own farm",
                    Data = null
                };
                return StatusCode(StatusCodes.Status403Forbidden, forbiddenResponse);
            }

            var response = await _workerAttendanceService.GetWorkerAttendanceByFarmer(
                farmerProfileId,
                workerProfileId,
                startDate,
                endDate);

            var apiResponse = new ApiResponse<List<WorkerAttendanceDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Worker attendance records retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving worker {WorkerProfileId} attendance for farmer {FarmerProfileId}", workerProfileId, farmerProfileId);

            if (ex.Message.Contains("no accepted applications"))
            {
                var notFoundResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Data = null
                };
                return NotFound(notFoundResponse);
            }

            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
        }
    }
}
