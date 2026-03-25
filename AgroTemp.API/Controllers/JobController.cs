using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.DTO.Job.JobDetail;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPut(ApiEndpointConstants.Job.UpdateJobPostStatusEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobPostDTO>> UpdateJobPostStatus([FromRoute] string id, [FromQuery] string status)
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
    public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetFilteredJobPosts([FromQuery] string? title, [FromQuery] string? category, [FromQuery] string? address, [FromQuery] string? skill)
    {
        try
        {
            var response = await _jobPostService.GetFilteredJobPosts(title, category, address, skill);
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

    // Job Detail Endpoints

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
    [ProducesResponseType(typeof(ApiResponse<JobDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobDetailDTO>> GetJobDetailById([FromRoute] string id)
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
            var apiResponse = new ApiResponse<JobDetailDTO>
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
}
