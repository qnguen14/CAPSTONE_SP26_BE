using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Skill;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize]
public class SkillController : ControllerBase
{
    private readonly ILogger<SkillController> _logger;
    private readonly ISkillService _skillService;

    public SkillController(ILogger<SkillController> logger, ISkillService skillService)
    {
        _logger = logger;
        _skillService = skillService;
    }

    [HttpGet(ApiEndpointConstants.Skill.GetAllSkillsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SkillResponse>>> GetAllSkills()
    {
        try
        {
            var response = await _skillService.GetAllSkills();

            var apiResponse = new ApiResponse<IEnumerable<SkillResponse>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Skills retrieved successfully",
                Data = response
            };

            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving skills");
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet(ApiEndpointConstants.Skill.GetSkillsByCategoryPagedEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SkillResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResponse<SkillResponse>>> GetSkillsByCategoryPaged([FromRoute] Guid categoryId, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        try
        {
            var response = await _skillService.GetSkillsByCategoryPagedAsync(categoryId, page, limit);

            var apiResponse = new ApiResponse<PaginatedResponse<SkillResponse>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Skills by category retrieved successfully",
                Data = response
            };

            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving skills for category {CategoryId}", categoryId);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet(ApiEndpointConstants.Skill.GetSkillByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<SkillResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SkillResponse>> GetSkillById([FromRoute] Guid id)
    {
        try
        {
            var response = await _skillService.GetSkillById(id);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Skill not found",
                    Data = null
                });
            }

            var apiResponse = new ApiResponse<SkillResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Skill retrieved successfully",
                Data = response
            };

            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving skill {SkillId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpPost(ApiEndpointConstants.Skill.CreateSkillEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<SkillResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<ActionResult<SkillResponse>> CreateSkill([FromBody] CreateSkillRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid request data",
                    Data = ModelState
                });
            }

            var response = await _skillService.CreateSkill(request);

            var apiResponse = new ApiResponse<SkillResponse>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Skill created successfully",
                Data = response
            };

            return StatusCode(StatusCodes.Status201Created, apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating skill");
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpPut(ApiEndpointConstants.Skill.UpdateSkillEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<SkillResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SkillResponse>> UpdateSkill([FromRoute] Guid id, [FromBody] UpdateSkillRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid request data",
                    Data = ModelState
                });
            }


            var response = await _skillService.UpdateSkill(id, request);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Skill not found",
                    Data = null
                });
            }

            var apiResponse = new ApiResponse<SkillResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Skill updated successfully",
                Data = response
            };

            return Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating skill {id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpDelete(ApiEndpointConstants.Skill.DeleteSkillEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteSkill([FromRoute] Guid id)
    {
        try
        {
            var result = await _skillService.DeleteSkill(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Skill not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Skill deleted successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting skill {SkillId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
}
