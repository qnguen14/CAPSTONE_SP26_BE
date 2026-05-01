using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Job.JobDetail;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class JobDetailController : ControllerBase
{
    private readonly ILogger<JobDetailController> _logger;
    private readonly IJobDetailService _jobDetailService;

    public JobDetailController(
        ILogger<JobDetailController> logger,
        IJobDetailService jobDetailService)
    {
        _logger = logger;
        _jobDetailService = jobDetailService;
    }

    // [HttpGet("{id}")]
    // [ProducesResponseType(typeof(ApiResponse<JobDetailResponseDTO>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    // [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin job detail by id.")]
    // [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get job detail by id.")]
    // [Microsoft.AspNetCore.Routing.EndpointName("JobDetailGetJobDetailById")]
    // public async Task<ActionResult<JobDetailResponseDTO>> GetJobDetailById([FromRoute] string id)
    // {
    //     try
    //     {
    //         var response = await _jobDetailService.GetById(id);
    //         if (response == null)
    //         {
    //             return NotFound(new ApiResponse<object>
    //             {
    //                 StatusCode = StatusCodes.Status404NotFound,
    //                 Message = "Job detail not found",
    //                 Data = null
    //             });
    //         }
    //         var apiResponse = new ApiResponse<JobDetailResponseDTO>
    //         {
    //             StatusCode = StatusCodes.Status200OK,
    //             Message = "Job detail retrieved successfully",
    //             Data = response
    //         };
    //         return Ok(apiResponse);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error retrieving job detail");
    //         return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
    //         {
    //             StatusCode = StatusCodes.Status500InternalServerError,
    //             Message = "An error occurred while retrieving job detail",
    //             Data = null
    //         });
    //     }
    // }

    [HttpGet(ApiEndpointConstants.Job.GetAllJobDetailsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobDetailDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobDetailDTO>>> GetAllJobDetails()
    {
        try
        {
            var response = await _jobDetailService.GetAllJobDetails();

            var apiResponse = new ApiResponse<IEnumerable<JobDetailDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job details retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job details");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job details",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobDetailByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobDetailResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobDetailResponseDTO>> GetJobDetailById([FromRoute] string id)
    {
        try
        {
            var response = await _jobDetailService.GetJobDetailById(id);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job detail not found",
                    Data = null
                });
            }
            var apiResponse = new ApiResponse<JobDetailResponseDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job detail retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job detail");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job detail",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Job.CreateJobDetailEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobDetailDTO>> CreateJobDetail([FromBody] CreateJobDetailRequest request)
    {
        try
        {
            var response = await _jobDetailService.CreateJobDetail(request);
            var apiResponse = new ApiResponse<JobDetailDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job detail created successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job detail");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while creating job detail",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobDetailEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobDetailDTO>> UpdateJobDetail([FromRoute] Guid id, [FromBody] UpdateJobDetailRequest request)
    {
        try
        {
            var response = await _jobDetailService.UpdateJobDetail(id, request);
            var apiResponse = new ApiResponse<JobDetailDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job detail updated successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job detail");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while updating job detail",
                Data = null
            });
        }
    }

    [HttpDelete(ApiEndpointConstants.Job.DeleteJobDetailEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteJobDetail([FromRoute] string id)
    {
        try
        {
            var result = await _jobDetailService.DeleteJobDetail(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job detail not found",
                    Data = null
                });
            }
            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job detail deleted successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job detail");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while deleting the job detail",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobDetailStatusEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobDetailDTO>> UpdateJobDetailStatus([FromRoute] string id, [FromQuery] string status)
    {
        try
        {
            var response = await _jobDetailService.UpdateJobDetailStatus(id, status);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job detail not found",
                    Data = null
                });
            }
            var apiResponse = new ApiResponse<JobDetailDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job detail status updated successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job detail status");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while updating job detail status",
                Data = null
            });
        }
    }


    [HttpPost(ApiEndpointConstants.Job.ReportDailyWorkerEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobDetailResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Bao cao cong viec hang ngay.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang report daily work.")]
    [Microsoft.AspNetCore.Routing.EndpointName("JobDetailReportDailyWork")]
    public async Task<ActionResult<JobDetailResponseDTO>> ReportDailyWork([FromRoute] Guid id, [FromBody] CreateDailyReportRequest request)
    {
        try
        {
            var response = await _jobDetailService.ReportDailyWork(id, request);
            var apiResponse = new ApiResponse<JobDetailResponseDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Daily work reported successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting daily work");
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobDetailByWorker)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<JobDetailResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin job details by worker id.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get job details by worker id.")]
    [Microsoft.AspNetCore.Routing.EndpointName("JobDetailGetJobDetailsByWorkerId")]
    public async Task<ActionResult<PaginatedResponse<JobDetailResponseDTO>>> GetJobDetailsByWorkerId(
        [FromRoute] Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        try
        {
            var response = await _jobDetailService.GetJobDetailsByWorkerId(id, page, limit);
            var apiResponse = new ApiResponse<PaginatedResponse<JobDetailResponseDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job details retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job details by worker");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job details",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobDetailByJobPost)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<JobDetailResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin job details by job post id.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get job details by job post id.")]
    [Microsoft.AspNetCore.Routing.EndpointName("JobDetailGetJobDetailsByJobPostId")]
    public async Task<ActionResult<PaginatedResponse<JobDetailResponseDTO>>> GetJobDetailsByJobPostId(
        [FromRoute] Guid id,
        [FromQuery] JobStatus? jobStatus,
        [FromQuery] bool orderByDescending = true,  
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        try
        {
            var response = await _jobDetailService.GetJobDetailsByJobPostId(id, jobStatus, orderByDescending, page, limit);
            var apiResponse = new ApiResponse<PaginatedResponse<JobDetailResponseDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job details retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job details by job post");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job details",
                Data = null
            });
        }
    }

    [Authorize(Roles = "Farmer")]
    [HttpPost(ApiEndpointConstants.Job.ApproveJobDetailEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobDetailResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Phe duyet job detail.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang approve job detail.")]
    [Microsoft.AspNetCore.Routing.EndpointName("JobDetailApproveJobDetail")]
    public async Task<ActionResult<JobDetailResponseDTO>> ApproveJobDetail([FromRoute] Guid id, [FromBody] ApproveJobDetailRequest request)
    {
        try
        {
            var response = await _jobDetailService.ApproveJobDetail(id, request);
            var apiResponse = new ApiResponse<JobDetailResponseDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job detail approved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving job detail");
            if (ex.Message.Contains("not found"))
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
    }
}
