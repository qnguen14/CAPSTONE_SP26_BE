using System.Security.Claims;
using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.DisputeReport;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Implements;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
public class DisputeReportController : ControllerBase
{
    private readonly ILogger<DisputeReportController> _logger;
    private readonly IDisputeReportService _disputeReportService;
    private readonly IUserService _userService;

    public DisputeReportController(
        ILogger<DisputeReportController> logger,
        IDisputeReportService disputeReportService,
        IUserService userService)
    {
        _logger = logger;
        _disputeReportService = disputeReportService;
        _userService = userService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet(ApiEndpointConstants.Dispute.GetAllDisputesEndpoint)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CustomDisputeReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomDisputeReportDTO>> GetAllDisputes()
    {
        try
        {
            var response = await _disputeReportService.GetAllDisputesAsync();
            return Ok(new ApiResponse<CustomDisputeReportDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Disputes retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving disputes");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving disputes",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.User.WarnUserEndpoint)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> WarnUser([FromRoute] Guid id)
    {
        try
        {
            await _userService.WarnUserAsync(id);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error warning user {UserId}", id);
            var statusCode = ex.Message.Contains("not found") ? StatusCodes.Status404NotFound : StatusCodes.Status500InternalServerError;
            return StatusCode(statusCode, new { message = ex.Message });
        }
    }

    [HttpGet(ApiEndpointConstants.Dispute.GetMyDisputesEndpoint)]
    [Authorize(Roles = "Farmer,Worker")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DisputeReportDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DisputeReportDTO>>> GetMyDisputes()
    {
        try
        {
            var response = await _disputeReportService.GetMyDisputesAsync(CurrentUserId);
            return Ok(new ApiResponse<IEnumerable<DisputeReportDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Your disputes retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user disputes");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving your disputes",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Dispute.GetDisputeSummaryEndpoint)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DisputeStatusSummaryDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DisputeStatusSummaryDTO>>> GetDisputeSummary()
    {
        try
        {
            var response = await _disputeReportService.GetDisputeCountsByStatusAsync();
            return Ok(new ApiResponse<IEnumerable<DisputeStatusSummaryDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Dispute summary retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dispute summary");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving dispute summary",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Dispute.GetDisputeByIdEndpoint)]
    [Authorize(Roles = "Admin,Farmer,Worker")]
    [ProducesResponseType(typeof(ApiResponse<DisputeReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DisputeReportDTO>> GetDisputeById([FromRoute] Guid id)
    {
        try
        {
            var isAdmin = User.IsInRole("Admin");
            var response = await _disputeReportService.GetDisputeByIdAsync(id, CurrentUserId, isAdmin);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Dispute not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<DisputeReportDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Dispute retrieved successfully",
                Data = response
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Forbidden access to dispute {DisputeId}", id);
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dispute {DisputeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving the dispute",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Dispute.CreateDisputeEndpoint)]
    [Authorize(Roles = "Farmer,Worker")]
    [ProducesResponseType(typeof(ApiResponse<DisputeReportDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DisputeReportDTO>> CreateDispute([FromBody] CreateDisputeReportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid dispute request data",
                Data = ModelState
            });
        }

        try
        {
            var response = await _disputeReportService.CreateDisputeAsync(CurrentUserId, request);
            return CreatedAtAction(
                nameof(GetDisputeById),
                new { id = response.Id },
                new ApiResponse<DisputeReportDTO>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Dispute created successfully",
                    Data = response
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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dispute");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while creating the dispute",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Dispute.UpdateDisputeEndpoint)]
    [Authorize(Roles = "Farmer,Worker")]
    [ProducesResponseType(typeof(ApiResponse<DisputeReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DisputeReportDTO>> UpdateDispute([FromRoute] Guid id, [FromBody] UpdateDisputeReportRequest request)
    {
        try
        {
            var response = await _disputeReportService.UpdateDisputeAsync(id, CurrentUserId, request);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Dispute not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<DisputeReportDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Dispute updated successfully",
                Data = response
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dispute {DisputeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while updating the dispute",
                Data = null
            });
        }
    }

    [HttpDelete(ApiEndpointConstants.Dispute.DeleteDisputeEndpoint)]
    [Authorize(Roles = "Admin,Farmer,Worker")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteDispute([FromRoute] Guid id)
    {
        try
        {
            var isAdmin = User.IsInRole("Admin");
            var result = await _disputeReportService.DeleteDisputeAsync(id, CurrentUserId, isAdmin);
            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Dispute not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Dispute deleted successfully",
                Data = null
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting dispute {DisputeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while deleting the dispute",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Dispute.ReviewDisputeEndpoint)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DisputeReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DisputeReportDTO>> ReviewDispute([FromRoute] Guid id, [FromBody] ReviewDisputeReportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid review request",
                Data = ModelState
            });
        }

        try
        {
            var response = await _disputeReportService.ReviewDisputeAsync(id, CurrentUserId, request);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Dispute not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<DisputeReportDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Dispute marked as under review",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing dispute {DisputeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while reviewing the dispute",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Dispute.ResolveDisputeEndpoint)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DisputeReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DisputeReportDTO>> ResolveDispute([FromRoute] Guid id, [FromBody] ResolveDisputeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid resolve request",
                Data = ModelState
            });
        }

        try
        {
            var response = await _disputeReportService.ResolveDisputeAsync(id, CurrentUserId, request);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Dispute not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<DisputeReportDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Dispute resolved successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving dispute {DisputeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while resolving the dispute",
                Data = null
            });
        }
    }

    [HttpPut(ApiEndpointConstants.Dispute.UpdateDisputeStatusEndpoint)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DisputeReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DisputeReportDTO>> UpdateDisputeStatus([FromRoute] Guid id, [FromBody] UpdateDisputeStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid status request",
                Data = ModelState
            });
        }

        try
        {
            var response = await _disputeReportService.UpdateDisputeStatusAsync(id, CurrentUserId, request.StatusId);
            if (response == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Dispute not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<DisputeReportDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Dispute status updated successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dispute status {DisputeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while updating the dispute status",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Dispute.SearchJobPostsForEmbedEndpoint)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<JobPostEmbedDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JobPostEmbedDTO>>> SearchJobPostsForEmbed(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var results = await _disputeReportService.SearchJobPostsForEmbedAsync(search, page, pageSize);
            return Ok(new ApiResponse<IEnumerable<JobPostEmbedDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Job posts retrieved successfully",
                Data = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching job posts for embed");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while searching job posts",
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Dispute.DisputeEndpoint + "/{id:guid}/comments")]
    [Authorize(Roles = "Admin,Farmer,Worker")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DisputeReportCommentDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DisputeReportCommentDTO>>> GetDisputeComments([FromRoute] Guid id)
    {
        try
        {
            var isAdmin = User.IsInRole("Admin");
            var response = await _disputeReportService.GetDisputeCommentsAsync(id, CurrentUserId, isAdmin);
            return Ok(new ApiResponse<IEnumerable<DisputeReportCommentDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Comments retrieved successfully",
                Data = response
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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for dispute {DisputeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while retrieving the comments",
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Dispute.DisputeEndpoint + "/{id:guid}/comments")]
    [Authorize(Roles = "Admin,Farmer,Worker")]
    [ProducesResponseType(typeof(ApiResponse<DisputeReportCommentDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DisputeReportCommentDTO>> AddDisputeComment([FromRoute] Guid id, [FromBody] CreateDisputeReportCommentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid comment request",
                Data = ModelState
            });
        }

        try
        {
            var isAdmin = User.IsInRole("Admin");
            var response = await _disputeReportService.AddDisputeCommentAsync(id, CurrentUserId, isAdmin, request);
            return StatusCode(StatusCodes.Status201Created, new ApiResponse<DisputeReportCommentDTO>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Comment added successfully",
                Data = response
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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to dispute {DisputeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred while adding the comment",
                Data = null
            });
        }
    }
}
