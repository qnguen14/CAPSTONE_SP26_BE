using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
public class JobCategoryController : ControllerBase
{
    private readonly ILogger<JobCategoryController> _logger;
    private readonly IJobCategoryService _jobCategoryService;

    public JobCategoryController(
        ILogger<JobCategoryController> logger,
        IJobCategoryService jobCategoryService)
    {
        _logger = logger;
        _jobCategoryService = jobCategoryService;
    }

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
}
