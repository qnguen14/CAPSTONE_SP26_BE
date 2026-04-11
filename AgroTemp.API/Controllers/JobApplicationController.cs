using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
public class JobApplicationController : ControllerBase
{
    private readonly ILogger<JobApplicationController> _logger;
    private readonly IJobApplicationService _jobApplicationService;

    public JobApplicationController(
        ILogger<JobApplicationController> logger,
        IJobApplicationService jobApplicationService)
    {
        _logger = logger;
        _jobApplicationService = jobApplicationService;
    }

    [HttpGet(ApiEndpointConstants.Job.GetAllJobApplicationsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobApplicationDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobApplicationDTO>>> GetAllJobApplications()
    {
        try
        {
            var response = await _jobApplicationService.GetAllJobApplications();
            var apiResponse = new ApiResponse<IEnumerable<JobApplicationDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job applications retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job applications");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job applications",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobApplicationsByWorkerEndpoint)]
    [Authorize(Roles = "Worker")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobApplicationDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobApplicationDTO>>> GetJobApplicationsByWorker()
    {
        try
        {
            var response = await _jobApplicationService.GetJobApplicationsByWorker();
            var apiResponse = new ApiResponse<IEnumerable<JobApplicationDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job applications by worker retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job applications by worker");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job applications by worker",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobApplicationsByJobPostEndpoint)]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<JobApplicationDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResponse<JobApplicationDTO>>> GetJobApplicationsByJobPostId([FromRoute] Guid jobPostId, [FromQuery] bool? includeAll, [FromQuery] int? statusId,
                                                                                                       [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        try
        {
            var farmerProfileIdClaim = User.FindFirst("FarmerProfileId")?.Value;

            if (string.IsNullOrEmpty(farmerProfileIdClaim) || !Guid.TryParse(farmerProfileIdClaim, out var farmerProfileId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Farmer profile not found",
                    Data = null
                });
            }

            var response = await _jobApplicationService.GetJobApplicationsByJobPostId(jobPostId, farmerProfileId, statusId, includeAll ?? false, page, limit);

            return Ok(new ApiResponse<PaginatedResponse<JobApplicationDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job applications retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job applications by job post id");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobApplicationsByFarmer)]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<JobApplicationDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResponse<JobApplicationDTO>>> GetJobApplicationsByFarmer([FromQuery] int statusId, [FromQuery] bool includeAll = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _jobApplicationService.GetJobApplicationsByFarmer(statusId, includeAll, pageNumber, pageSize);
            var apiResponse = new ApiResponse<PaginatedResponse<JobApplicationDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job applications by farmer retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job applications by farmer");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job applications by farmer",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobApplicationByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobApplicationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobApplicationDTO>> GetJobApplicationById([FromRoute] string id)
    {
        try
        {
            var response = await _jobApplicationService.GetJobApplicationById(id);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job application not found",
                    Data = null
                });
            }
            var apiResponse = new ApiResponse<JobApplicationDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job application retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job application");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving the job application",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Job.CreateJobApplicationEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobApplicationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobApplicationDTO>> CreateJobApplication([FromBody] CreateJobApplicationRequest request)
    {
        try
        {
            var response = await _jobApplicationService.CreateJobApplication(request);
            var apiResponse = new ApiResponse<JobApplicationDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job application created successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job application");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while creating the job application",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobApplicationEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobApplicationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobApplicationDTO>> UpdateJobApplication([FromRoute] Guid id, [FromBody] UpdateJobApplicationRequest request)
    {
        try
        {
            var response = await _jobApplicationService.UpdateJobApplication(id, request);
            var apiResponse = new ApiResponse<JobApplicationDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job application updated successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job application");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while updating the job application",
                Data = null
            });
        }
    }

    [HttpDelete(ApiEndpointConstants.Job.DeleteJobApplicationEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteJobApplication([FromRoute] string id)
    {
        try
        {
            var result = await _jobApplicationService.DeleteJobApplication(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job application not found",
                    Data = null
                });
            }
            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job application deleted successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job application");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while deleting the job application",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.RespondJobApplicationEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobApplicationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobApplicationDTO>> RespondJobApplication([FromRoute] string id, [FromBody] RespondJobApplicationRequest request)
    {
        try
        {
            var response = await _jobApplicationService.RespondJobApplication(id, request);
            var apiResponse = new ApiResponse<JobApplicationDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job application responded successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting job applications");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while responding to the job application",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.AutoAcceptUrgentJobApplicationsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobApplicationDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobApplicationDTO>>> AutoAcceptUrgentJobApplications([FromBody] List<Guid> jobApplicationIds)
    {
        try
        {
            var response = await _jobApplicationService.AutoAcceptUrgentJobApplicationsAsync(jobApplicationIds);
            return Ok(new ApiResponse<IEnumerable<JobApplicationDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"{response.Count} pending application(s) have been automatically accepted.",
                Data = response
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument for auto-accept urgent job applications");
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found during auto-accept urgent job applications");
            return NotFound(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = ex.Message,
                Data = null
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized auto-accept attempt");
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid auto-accept operation");
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error auto-accepting urgent job applications");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while auto-accepting job applications",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.CancelJobApplicationEndpoint)]
    [Authorize(Roles = "Worker")]
    [ProducesResponseType(typeof(ApiResponse<JobApplicationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobApplicationDTO>> CancelJobApplication([FromRoute] Guid id)
    {
        try
        {
            var response = await _jobApplicationService.CancelJobApplication(id);

            return Ok(new ApiResponse<JobApplicationDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job application cancelled successfully",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Cancel job application: Not found");
            return NotFound(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = ex.Message,
                Data = null
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Cancel job application: Unauthorized");
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cancel job application: Invalid state");
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling job application");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while cancelling the job application",
                Data = null
            });
        }
    }
}
