using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Job.JobDetail;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
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

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<JobDetailResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobDetailResponseDTO>> GetJobDetailById([FromRoute] string id)
    {
        try
        {
            var response = await _jobDetailService.GetById(id);
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

    [HttpPost("report-daily")]
    [ProducesResponseType(typeof(ApiResponse<JobDetailResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobDetailResponseDTO>> ReportDailyWork([FromBody] CreateDailyReportRequest request)
    {
        try
        {
            var response = await _jobDetailService.ReportDailyWork(request);
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

    [HttpGet("worker/{workerId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobDetailResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobDetailResponseDTO>>> GetJobDetailsByWorkerId([FromRoute] Guid workerId)
    {
        try
        {
            var response = await _jobDetailService.GetJobDetailsByWorkerId(workerId);
            var apiResponse = new ApiResponse<IEnumerable<JobDetailResponseDTO>>
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

    [HttpGet("job-post/{jobPostId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobDetailResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobDetailResponseDTO>>> GetJobDetailsByJobPostId([FromRoute] Guid jobPostId)
    {
        try
        {
            var response = await _jobDetailService.GetJobDetailsByJobPostId(jobPostId);
            var apiResponse = new ApiResponse<IEnumerable<JobDetailResponseDTO>>
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

    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(ApiResponse<JobDetailResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
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