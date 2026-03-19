using AgroTemp.API.Constants;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PayOS.Models.Webhooks;

namespace AgroTemp.API.Controllers;

[ApiController]
public class PaymentWebhookController : ControllerBase
{
    private readonly IPayOSService _payOSService;
    private readonly ILogger<PaymentWebhookController> _logger;

    public PaymentWebhookController(IPayOSService payOSService, ILogger<PaymentWebhookController> logger)
    {
        _payOSService = payOSService;
        _logger = logger;
    }

    [HttpPost(ApiEndpointConstants.Payment.VerifyWebhookEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> VerifyPayment([FromBody] Webhook webhook)
    {
        if (webhook == null)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Webhook data is required",
                Data = null
            });
        }

        try
        {
            var result = await _payOSService.VerifyWebhookAsync(webhook);
            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = result.Message,
                Data = new { result.OrderCode }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook processing error");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Webhook processing failed",
                Data = ex.Message
            });
        }
    }
}
