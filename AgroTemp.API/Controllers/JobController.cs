using AgroTemp.API.Constants;
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

    public JobController(
        ILogger<JobController> logger,
        IJobCategoryService jobCategoryService,
        IJobPostService jobPostService)
    {
        _logger = logger;
        _jobCategoryService = jobCategoryService;
        _jobPostService = jobPostService;
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
    public async Task<ActionResult<JobCategoryDTO>> GetJobCategoryById([FromQuery] string id)
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
    public async Task<ActionResult<JobCategoryDTO>> UpdateJobCategory([FromBody] UpdateJobCategoryRequest request)
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
}
