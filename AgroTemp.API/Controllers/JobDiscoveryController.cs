using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
public class JobDiscoveryController : ControllerBase
{
    private readonly ILogger<JobDiscoveryController> _logger;
    private readonly IJobPostService _jobPostService;

    public JobDiscoveryController(
        ILogger<JobDiscoveryController> logger,
        IJobPostService jobPostService)
    {
        _logger = logger;
        _jobPostService = jobPostService;
    }

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
}
