using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Payment;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;

namespace AgroTemp.API.Controllers;

[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPayOSService _payOSService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPayOSService payOSService, ILogger<PaymentController> logger)
    {
        _payOSService = payOSService;
        _logger = logger;
    }

    [HttpGet(ApiEndpointConstants.Payment.GetOrderEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<PayOSOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PayOSOrderResponse>> Get([FromRoute] Guid id)
    {
        try
        {
            var order = await _payOSService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Order not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<PayOSOrderResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Order retrieved successfully",
                Data = order
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve order {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = $"Failed to retrieve order {id}",
                Data = ex.Message
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Payment.CreateOrderEndpoint)]
    //[Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<PayOSOrderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PayOSOrderResponse>> CreatePayment([FromBody] CreatePayOSOrderRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Order data is required",
                Data = null
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid order data",
                Data = ModelState
            });
        }

        try
        {
            var order = await _payOSService.CreatePaymentAsync(request);
            return StatusCode(StatusCodes.Status201Created, new ApiResponse<PayOSOrderResponse>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Order created successfully",
                Data = order
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to create order",
                Data = ex.Message
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Payment.CancelOrderEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<PaymentLink>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentLink>> CancelPayment([FromRoute] Guid id, [FromQuery] string? cancellationReason)
    {
        try
        {
            var paymentLink = await _payOSService.CancelPaymentAsync(id, cancellationReason);
            if (paymentLink == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Order not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<PaymentLink>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Order cancelled successfully",
                Data = paymentLink
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel order {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = $"Failed to cancel order {id}",
                Data = ex.Message
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Payment.GetOrderInvoicesEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<PayOSInvoicesInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PayOSInvoicesInfoResponse>> GetInvoices([FromRoute] Guid id)
    {
        try
        {
            var invoices = await _payOSService.GetInvoicesAsync(id);
            if (invoices == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Order not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<PayOSInvoicesInfoResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Invoices retrieved successfully",
                Data = invoices
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve invoices for order {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = $"Failed to retrieve invoices for order {id}",
                Data = ex.Message
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Payment.DownloadOrderInvoiceEndpoint)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DownloadInvoice([FromRoute] Guid id, [FromRoute] string invoiceId)
    {
        try
        {
            var file = await _payOSService.DownloadInvoiceAsync(id, invoiceId);
            if (file == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Invoice not found for this order",
                    Data = null
                });
            }

            return File(file.Value.Content, file.Value.ContentType, file.Value.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download invoice {InvoiceId} for order {OrderId}", invoiceId, id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = $"Failed to download invoice {invoiceId}",
                Data = ex.Message
            });
        }
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
