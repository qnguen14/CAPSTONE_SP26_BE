using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
public class JobPostController : ControllerBase
{
    private readonly ILogger<JobPostController> _logger;
    private readonly IJobPostService _jobPostService;

    public JobPostController(
        ILogger<JobPostController> logger,
        IJobPostService jobPostService)
    {
        _logger = logger;
        _jobPostService = jobPostService;
    }

    [HttpGet(ApiEndpointConstants.Job.GetAllJobPostsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetAllJobPosts()
    {
        try
        {
            var response = await _jobPostService.GetAllJobPosts();

            var apiResponse = new ApiResponse<IEnumerable<JobPostDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job posts retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job posts");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job posts",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobPostByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobPostDTO>> GetJobPostById([FromRoute] string id)
    {
        try
        {
            var response = await _jobPostService.GetJobPostById(id);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job post not found",
                    Data = null
                });
            }
            var apiResponse = new ApiResponse<JobPostDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job post retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job post");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving the job post",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobPostsByFarmerEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetJobPostsByFarmer()
    {
        try
        {
            var response = await _jobPostService.GetJobPostsByFarmerId();
            var apiResponse = new ApiResponse<IEnumerable<JobPostDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job posts by farmer retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job posts by farmer");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job posts by farmer",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetFarmerJobHistoryEndpoint)]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetFarmerJobHistory()
    {
        try
        {
            var response = await _jobPostService.GetFarmerJobHistory();
            var apiResponse = new ApiResponse<IEnumerable<JobPostDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farmer job history retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to farmer job history");
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving farmer job history");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving farmer job history",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobPostsByStatusEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetJobPostsByStatus([FromRoute] JobPostStatus status)
    {
        try
        {
            var response = await _jobPostService.GetJobPostsByStatus(status);
            var apiResponse = new ApiResponse<IEnumerable<JobPostDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job posts by status retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job posts by status");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job posts by status",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Job.CreateJobPostEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobPostDTO>> CreateJobPost([FromBody] CreateJobPostRequest request)
    {
        try
        {
            var response = await _jobPostService.CreateJobPost(request);
            var apiResponse = new ApiResponse<JobPostDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job post created successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized create job post attempt");
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid create job post request");
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }

        catch (Exception ex)
        {
            if (ex.Message != null && ex.Message.Contains("Insufficient wallet balance to create job post") || ex is UnauthorizedAccessException)
            {
                _logger.LogWarning(ex, "Insufficient wallet balance for job post");
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = null
                });
            }
            _logger.LogError(ex, "Error creating job post");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while creating job post",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobPostEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobPostDTO>> UpdateJobPost([FromRoute] Guid id, [FromBody] UpdateJobPostRequest request)
    {
        try
        {
            var response = await _jobPostService.UpdateJobPost(id, request);
            var apiResponse = new ApiResponse<JobPostDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job post updated successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job post");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while updating the job post",
                Data = null
            });
        }
    }

    [HttpDelete(ApiEndpointConstants.Job.DeleteJobPostEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteJobPost([FromRoute] string id)
    {
        try
        {
            var result = await _jobPostService.DeleteJobPost(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job post not found",
                    Data = null
                });
            }
            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job post deleted successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job post");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while deleting the job post",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Job.CancelJobPostEndpoint)]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobPostDTO>> CancelJobPost([FromRoute] Guid id)
    {
        try
        {
            var response = await _jobPostService.CancelJobPost(id);

            var apiResponse = new ApiResponse<JobPostDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job post cancelled successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Cancel job post: Not found");
            return NotFound(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = ex.Message,
                Data = null
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Cancel job post: Unauthorized");
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cancel job post: Invalid state");
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling job post");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while cancelling the job post",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobPostStatusEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobPostDTO>> UpdateJobPostStatus([FromRoute] string id, JobPostStatus status)
    {
        try
        {
            var response = await _jobPostService.UpdateJobPostStatus(id, status);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job post not found",
                    Data = null
                });
            }
            var apiResponse = new ApiResponse<JobPostDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job post status updated successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job post status");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while updating the job post status",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobPostUrgencyEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobPostDTO>> UpdateJobPostUrgency([FromRoute] string id, [FromQuery] bool isUrgent)
    {
        try
        {
            var response = await _jobPostService.UpdateJobPostUrgency(id, isUrgent);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job post not found",
                    Data = null
                });
            }
            var apiResponse = new ApiResponse<JobPostDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job post urgency updated successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job post urgency");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while updating the job post urgency",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetFilteredJobPostsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetFilteredJobPosts([FromQuery] string? title, [FromQuery] string? category, [FromQuery] string? address, [FromQuery] List<string>? skill, [FromQuery] bool? sortByDatesDescending)
    {
        try
        {
            var response = await _jobPostService.GetFilteredJobPosts(title, category, address, skill, sortByDatesDescending ?? true);
            var apiResponse = new ApiResponse<IEnumerable<JobPostDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Filtered job posts retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving filtered job posts");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving filtered job posts",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetFilteredJobPostsByFarmerEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<JobPostDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResponse<JobPostDTO>>> GetFilteredJobPostsByFarmer([FromQuery] string? title, 
    [FromQuery] string? category, [FromQuery] string? address, [FromQuery] List<string>? skill, [FromQuery] bool? sortByDatesDescending, [FromQuery] JobType? jobType = null, [FromQuery] JobStatus? jobStatus = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        try
        {
            var response = await _jobPostService.GetFilteredJobPostsByFarmer(title, category, address, skill, sortByDatesDescending ?? true, jobType, jobStatus, page, limit);
            var apiResponse = new ApiResponse<PaginatedResponse<JobPostDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Filtered job posts retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving filtered job posts");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving filtered job posts",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Job.SaveJobPostDraftEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobPostDTO>> SaveJobPostDraft([FromBody] CreateJobPostRequest request)
    {
        try
        {
            var response = await _jobPostService.SaveJobPostDraft(request);
            var apiResponse = new ApiResponse<JobPostDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job post draft saved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized save job post draft attempt");
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid save job post draft request");
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving job post draft");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while saving the job post draft",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetFarmerDraftsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetFarmerDrafts([FromQuery] JobType? jobType = null)
    {
        try
        {
            var response = await _jobPostService.GetFarmerDrafts(jobType);
            var apiResponse = new ApiResponse<IEnumerable<JobPostDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Farmer draft job posts retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized get farmer drafts attempt");
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving farmer draft job posts");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving farmer draft job posts",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetAcceptedWorkersPerDayEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WorkersPerDayDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<WorkersPerDayDTO>>> GetAcceptedWorkersPerDay([FromRoute] Guid id)
    {
        try
        {
            var response = await _jobPostService.GetAcceptedWorkersPerDayAsync(id);
            var apiResponse = new ApiResponse<IEnumerable<WorkersPerDayDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Accepted workers per day retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accepted workers per day for job post {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving accepted workers per day",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Job.ToggleSaveJobPostEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<SavedJobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SavedJobPostDTO>> ToggleSaveJobPost([FromRoute] Guid id)
    {
        try
        {
            var response = await _jobPostService.ToggleSaveJobPostAsync(id);
            if (response == null)
            {
                return Ok(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Job post unsaved successfully",
                    Data = null
                });
            }
            return Ok(new ApiResponse<SavedJobPostDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job post saved successfully",
                Data = response
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized toggle save job post attempt");
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling save for job post {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while saving the job post",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetSavedJobPostsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SavedJobPostDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SavedJobPostDTO>>> GetSavedJobPosts()
    {
        try
        {
            var response = await _jobPostService.GetSavedJobPostsAsync();
            return Ok(new ApiResponse<IEnumerable<SavedJobPostDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Saved job posts retrieved successfully",
                Data = response
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized get saved job posts attempt");
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving saved job posts");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving saved job posts",
                Data = null
            });
        }
    }
}
