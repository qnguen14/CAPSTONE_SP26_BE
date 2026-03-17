using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.DTO.Job.JobCategory;
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

    public JobController(
        ILogger<JobController> logger,
        IJobCategoryService jobCategoryService,
        IJobPostService jobPostService,
        IJobApplicationService jobApplicationService)
    {
        _logger = logger;
        _jobCategoryService = jobCategoryService;
        _jobPostService = jobPostService;
        _jobApplicationService = jobApplicationService;
    }

    [HttpGet(ApiEndpointConstants.Job.GetAllJobCategoriesEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobCategory>>), StatusCodes.Status200OK)]
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobCategoryEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobCategoryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobCategoryDTO>> UpdateJobCategory([FromRoute] UpdateJobCategoryRequest request)
    {
        try
        {
            var response = await _jobCategoryService.UpdateJobCategory(request);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetAllJobPostsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPost>>), StatusCodes.Status200OK)]
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobPostEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobPostDTO>> UpdateJobPost([FromRoute] UpdateJobPostRequest request)
    {
        try
        {
            var response = await _jobPostService.UpdateJobPost(request);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet(ApiEndpointConstants.Job.GetAllJobApplicationsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobApplication>>), StatusCodes.Status200OK)]
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpPost(ApiEndpointConstants.Job.CreateJobApplicationEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobPostDTO>), StatusCodes.Status200OK)]
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpPut(ApiEndpointConstants.Job.UpdateJobApplicationEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<JobApplicationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobApplicationDTO>> UpdateJobApplication([FromRoute] UpdateJobApplicationRequest request)
    {
        try
        {
            var response = await _jobApplicationService.UpdateJobApplication(request);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
}
