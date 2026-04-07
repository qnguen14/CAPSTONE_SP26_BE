using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.DTO.Job.JobDetail;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AgroTemp.API.Controllers;

[ApiController]
public class JobController : ControllerBase
{
    private readonly ILogger<JobController> _logger;
    private readonly IJobCategoryService _jobCategoryService;
    private readonly IJobPostService _jobPostService;
    private readonly IJobApplicationService _jobApplicationService;
    private readonly IJobDetailService _jobDetailService;

    public JobController(
        ILogger<JobController> logger,
        IJobCategoryService jobCategoryService,
        IJobPostService jobPostService,
        IJobApplicationService jobApplicationService,
        IJobDetailService jobDetailService)
    {
        _logger = logger;
        _jobCategoryService = jobCategoryService;
        _jobPostService = jobPostService;
        _jobApplicationService = jobApplicationService;
        _jobDetailService = jobDetailService;
    }

    //Job Category Endpoints

    [HttpGet(ApiEndpointConstants.Job.GetAllJobCategoriesEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobCategoryDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobCategoryDTO>>> GetAllJobCategories()
    {
        try
        {
            var response = await _jobCategoryService.GetAllJobCategories();

            var apiResponse = new ApiResponse<IEnumerable<JobCategoryDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job categories retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job categories");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving job categories",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobCategoryByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobCategoryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobCategoryDTO>> GetJobCategoryById([FromRoute] string id)
    {
        try
        {
            var response = await _jobCategoryService.GetJobCategoryById(id);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job category not found",
                    Data = null
                });
            }
            var apiResponse = new ApiResponse<JobCategoryDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job category retrieved successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job category");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving the job category",
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

            if(string.IsNullOrEmpty(farmerProfileIdClaim) || !Guid.TryParse(farmerProfileIdClaim, out var farmerProfileId))
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

    [HttpPost(ApiEndpointConstants.Job.CreateJobCategoryEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobCategoryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobCategoryDTO>> CreateJobCategory([FromBody] CreateJobCategoryRequest request)
    {
        try
        {
            var response = await _jobCategoryService.CreateJobCategory(request);
            var apiResponse = new ApiResponse<JobCategoryDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job category created successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job category");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while creating the job category",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobCategoryEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobCategoryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobCategoryDTO>> UpdateJobCategory([FromRoute] Guid id, [FromBody] UpdateJobCategoryRequest request)
    {
        try
        {
            var response = await _jobCategoryService.UpdateJobCategory(id, request);
            var apiResponse = new ApiResponse<JobCategoryDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job category updated successfully",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job category");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while updating the job category",
                Data = null
            });
        }
    }

    [HttpDelete(ApiEndpointConstants.Job.DeleteJobCategoryEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteJobCategory([FromRoute] string id)
    {
        try
        {
            var result = await _jobCategoryService.DeleteJobCategory(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Job category not found",
                    Data = null
                });
            }
            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job category deleted successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job category");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while deleting the job category",
                Data = null
            });
        }
    }

    // Job Post Endpoints

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
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetJobPostsByFarmer([FromRoute] Guid farmerId)
    {
        try
        {
            var response = await _jobPostService.GetJobPostsByFarmerId(farmerId);
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
            // Check for insufficient wallet balance error
            if (ex.Message != null && ex.Message.Contains("Insufficient wallet balance to create job post"))
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
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetFilteredJobPostsByFarmer([FromQuery] string? title, [FromQuery] string? category, [FromQuery] string? address, [FromQuery] List<string>? skill, [FromQuery] bool? sortByDatesDescending)
    {
        try
        {
            var response = await _jobPostService.GetFilteredJobPostsByFarmer(title, category, address, skill, sortByDatesDescending ?? true);
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
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetFarmerDrafts()
    {
        try
        {
            var response = await _jobPostService.GetFarmerDrafts();
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

    // Job Application Endpoints

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

    
    // Job Discovery and Search Endpoints

    [HttpPost(ApiEndpointConstants.Job.SearchJobsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedJobDiscoveryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PaginatedJobDiscoveryResponse>>> SearchJobs([FromBody] JobSearchFilterRequest filter)
    {
        try
        {
            var response = await _jobPostService.SearchJobsAsync(filter);
            var apiResponse = new ApiResponse<PaginatedJobDiscoveryResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = response.Message,
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching jobs");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while searching jobs",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetNearbyJobsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobDiscoveryDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobDiscoveryDTO>>>> GetNearbyJobs(
        [FromQuery] decimal latitude, 
        [FromQuery] decimal longitude, 
        [FromQuery] double? maxDistanceKm = 20)
    {
        try
        {
            if (latitude == 0 || longitude == 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Valid latitude and longitude are required",
                    Data = null
                });
            }

            var response = await _jobPostService.GetNearbyJobsAsync(latitude, longitude, maxDistanceKm ?? 20);
            var apiResponse = new ApiResponse<IEnumerable<JobDiscoveryDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Found {response.Count} nearby job(s)",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nearby jobs");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving nearby jobs",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobsByDateEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobDiscoveryDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobDiscoveryDTO>>>> GetJobsByDate([FromQuery] string dateFilter)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dateFilter))
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Date filter is required. Use: today, tomorrow, weekend, or upcoming",
                    Data = null
                });
            }

            var response = await _jobPostService.GetJobsByDateAsync(dateFilter);
            var apiResponse = new ApiResponse<IEnumerable<JobDiscoveryDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Found {response.Count} job(s) for {dateFilter}",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs by date");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving jobs by date",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobsBySkillEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobDiscoveryDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobDiscoveryDTO>>>> GetJobsBySkill([FromQuery] string skills)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(skills))
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Skills are required (comma-separated)",
                    Data = null
                });
            }

            var skillList = skills.Split(',').Select(s => s.Trim()).ToList();
            var response = await _jobPostService.GetJobsBySkillAsync(skillList);
            var apiResponse = new ApiResponse<IEnumerable<JobDiscoveryDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Found {response.Count} job(s) requiring skills: {skills}",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs by skill");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving jobs by skill",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobsByWageRangeEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobDiscoveryDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobDiscoveryDTO>>>> GetJobsByWageRange(
        [FromQuery] decimal minWage, 
        [FromQuery] decimal? maxWage = null)
    {
        try
        {
            if (minWage < 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Minimum wage must be greater than or equal to 0",
                    Data = null
                });
            }

            var response = await _jobPostService.GetJobsByWageRangeAsync(minWage, maxWage);
            var wageRangeMsg = maxWage.HasValue ? $"{minWage} - {maxWage}" : $"from {minWage}";
            var apiResponse = new ApiResponse<IEnumerable<JobDiscoveryDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Found {response.Count} job(s) with wages {wageRangeMsg}",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs by wage range");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving jobs by wage range",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetJobsByTypeEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobDiscoveryDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobDiscoveryDTO>>>> GetJobsByType([FromQuery] int jobTypeId)
    {
        try
        {
            if (jobTypeId < 1 || jobTypeId > 3)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid job type. Use: 1 (Daily), 2 (PerPlot), 3 (PerJob)",
                    Data = null
                });
            }

            var response = await _jobPostService.GetJobsByTypeAsync(jobTypeId);
            var jobTypeName = jobTypeId switch { 1 => "Daily", 2 => "PerPlot", 3 => "PerJob", _ => "Unknown" };
            var apiResponse = new ApiResponse<IEnumerable<JobDiscoveryDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Found {response.Count} {jobTypeName} job(s)",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs by type");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving jobs by type",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetUrgentJobsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobDiscoveryDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobDiscoveryDTO>>>> GetUrgentJobs(
        [FromQuery] decimal latitude, 
        [FromQuery] decimal longitude, 
        [FromQuery] double? maxDistanceKm = 20)
    {
        try
        {
            if (latitude == 0 || longitude == 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Valid latitude and longitude are required",
                    Data = null
                });
            }

            var response = await _jobPostService.GetUrgentJobsAsync(latitude, longitude, maxDistanceKm ?? 20);
            var apiResponse = new ApiResponse<IEnumerable<JobDiscoveryDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Found {response.Count} urgent job(s) nearby",
                Data = response
            };
            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving urgent jobs");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving urgent jobs",
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
