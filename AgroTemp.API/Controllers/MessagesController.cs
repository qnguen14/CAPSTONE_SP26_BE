using AgroTemp.API.Constants;
using AgroTemp.API.Hubs;
using AgroTemp.Domain.DTO.Message;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessagesController> _logger;
    private readonly IHubContext<ChatHub> _chatHubContext;

    public MessagesController(IMessageService messageService, ILogger<MessagesController> logger, IHubContext<ChatHub> chatHubContext)
    {
        _messageService = messageService;
        _logger = logger;
        _chatHubContext = chatHubContext;
    }

    [HttpGet(ApiEndpointConstants.Messages.GetMessagesEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<MessageDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<MessageDTO>>>> GetMessages(
        [FromQuery] Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        try
        {
            var messages = await _messageService.GetMessagesAsync(userId, page, limit);

            return Ok(new ApiResponse<PaginatedResponse<MessageDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Messages retrieved successfully",
                Data = messages
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving messages");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }

    [HttpPost(ApiEndpointConstants.Messages.SendMessageEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<MessageDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<MessageDTO>>> SendMessage([FromBody] CreateMessageRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Request is required",
                    Data = null
                });
            }

            if (request.JobPostId.HasValue && !User.IsInRole("Admin"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Only admins can attach a job post to a message.",
                    Data = null
                });
            }

            var message = await _messageService.SendMessageAsync(request.ReceiverId, request.Content, request.JobPostId);

            await _chatHubContext.Clients
               .Group(message.SenderId.ToString())
               .SendAsync("NewMessage", message);
            await _chatHubContext.Clients
                .Group(message.ReceiverId.ToString())
                .SendAsync("NewMessage", message);


            return Ok(new ApiResponse<MessageDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Message sent successfully",
                Data = message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }

    [HttpPatch(ApiEndpointConstants.Messages.MarkConversationAsReadEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<int>>> MarkConversationAsRead([FromBody] MarkConversationAsReadRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Request is required",
                    Data = null
                });
            }

            var updatedCount = await _messageService.MarkConversationAsReadAsync(request.SenderId);

            return Ok(new ApiResponse<int>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Messages marked as read",
                Data = updatedCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking conversation as read");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Messages.GetRecentConversationsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<List<ConversationDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<ConversationDTO>>>> GetRecentConversations()
    {
        try
        {
            var conversations = await _messageService.GetRecentConversationsAsync();

            return Ok(new ApiResponse<List<ConversationDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Conversations retrieved successfully",
                Data = conversations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversations");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }
}

