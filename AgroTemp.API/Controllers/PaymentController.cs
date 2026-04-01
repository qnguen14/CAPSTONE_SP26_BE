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
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin nap tien theo id.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang ay thong tin nap tien id.")]
    [Microsoft.AspNetCore.Routing.EndpointName("PaymentGet")]
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
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(typeof(ApiResponse<PayOSOrderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Tao moi nap tien.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang nap tien.")]
    [Microsoft.AspNetCore.Routing.EndpointName("PaymentCreatePayment")]
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
    [Microsoft.AspNetCore.Http.EndpointSummary("Huy nap tien.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang huy nap tien.")]
    [Microsoft.AspNetCore.Routing.EndpointName("PaymentCancelPayment")]
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

    [HttpGet(ApiEndpointConstants.Payment.CallbackEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<PayOSOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin nap tien khi payos callback")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang lấy thông tin nap tien khi payos callback.")]
    [Microsoft.AspNetCore.Routing.EndpointName("PaymentGetPaymentCallback")]
    public async Task<ActionResult<PayOSOrderResponse>> GetPaymentCallback([FromQuery] PaymentCallbackRequest request)
    {
        try
        {
            var order = await _payOSService.GetPaymentByCallbackAsync(request.OrderCode, request.Id);
            if (order == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Order not found for orderCode {request.OrderCode}",
                    Data = null
                });
            }

            return Ok(new ApiResponse<PayOSOrderResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Payment info retrieved successfully",
                Data = order
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve payment callback for orderCode {OrderCode}", request.OrderCode);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to retrieve payment info",
                Data = ex.Message
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Payment.VerifyWebhookEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Xac minh nap tien.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang verify nap tien dành cho payos.")]
    [Microsoft.AspNetCore.Routing.EndpointName("PaymentVerifyPayment")]
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
