using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.DisputeReport;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
public class DisputeReportController : ControllerBase
{
    private readonly ILogger<DisputeReportController> _logger;
    private readonly IDisputeReportService _disputeReportService;

    public DisputeReportController(
        ILogger<DisputeReportController> logger,
        IDisputeReportService disputeReportService)
    {
        _logger = logger;
        _disputeReportService = disputeReportService;
    }

    [HttpGet(ApiEndpointConstants.Dispute.GetAllDisputesEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DisputeReportDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DisputeReportDTO>>> GetAllDisputes()
    {
        try
        {
            var response = await _disputeReportService.GetAllDisputesAsync();
            return Ok(new ApiResponse<IEnumerable<DisputeReportDTO>>
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

    [HttpGet(ApiEndpointConstants.Dispute.GetDisputeByIdEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<DisputeReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DisputeReportDTO>> GetDisputeById([FromRoute] Guid id)
    {
        try
        {
            var response = await _disputeReportService.GetDisputeByIdAsync(id);
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
    [ProducesResponseType(typeof(ApiResponse<DisputeReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
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
            var response = await _disputeReportService.CreateDisputeAsync(request);
            return Ok(new ApiResponse<DisputeReportDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Dispute created successfully",
                Data = response
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
    [ProducesResponseType(typeof(ApiResponse<DisputeReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DisputeReportDTO>> UpdateDispute([FromRoute] Guid id, [FromBody] UpdateDisputeReportRequest request)
    {
        try
        {
            var response = await _disputeReportService.UpdateDisputeAsync(id, request);
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
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteDispute([FromRoute] Guid id)
    {
        try
        {
            var result = await _disputeReportService.DeleteDisputeAsync(id);
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
}
