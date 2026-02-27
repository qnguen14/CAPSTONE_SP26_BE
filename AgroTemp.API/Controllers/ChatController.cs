using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Chat;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    /// <summary>
    /// Send a message to another user
    /// </summary>
    [HttpPost(ApiEndpointConstants.Chat.SendMessageEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<ChatMessageDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ChatMessageDTO>> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var message = await _chatService.SendMessageAsync(request);

            var apiResponse = new ApiResponse<ChatMessageDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Message sent successfully",
                Data = message
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            };
            return Unauthorized(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            };
            return BadRequest(apiResponse);
        }
    }

    /// <summary>
    /// Get conversation with another user
    /// </summary>
    [HttpGet(ApiEndpointConstants.Chat.GetConversationEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<List<ChatMessageDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ChatMessageDTO>>> GetConversation(Guid otherUserId)
    {
        try
        {
            var messages = await _chatService.GetConversationAsync(otherUserId);

            var apiResponse = new ApiResponse<List<ChatMessageDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Conversation retrieved successfully",
                Data = messages
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            };
            return Unauthorized(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            };
            return BadRequest(apiResponse);
        }
    }

    /// <summary>
    /// Get all conversations for current user
    /// </summary>
    [HttpGet(ApiEndpointConstants.Chat.GetConversationsEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<List<ConversationPreviewDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ConversationPreviewDTO>>> GetConversations()
    {
        try
        {
            var conversations = await _chatService.GetConversationsAsync();

            var apiResponse = new ApiResponse<List<ConversationPreviewDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Conversations retrieved successfully",
                Data = conversations
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            };
            return Unauthorized(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversations");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            };
            return BadRequest(apiResponse);
        }
    }

    /// <summary>
    /// Mark a message as read
    /// </summary>
    [HttpPut(ApiEndpointConstants.Chat.MarkAsReadEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> MarkAsRead(Guid messageId)
    {
        try
        {
            await _chatService.MarkAsReadAsync(messageId);

            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Message marked as read",
                Data = null
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            };
            return Unauthorized(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as read");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            };
            return BadRequest(apiResponse);
        }
    }

    /// <summary>
    /// Get unread message count
    /// </summary>
    [HttpGet(ApiEndpointConstants.Chat.GetUnreadCountEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<UnreadCountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UnreadCountResponse>> GetUnreadCount()
    {
        try
        {
            var count = await _chatService.GetUnreadCountAsync();

            var apiResponse = new ApiResponse<UnreadCountResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Unread count retrieved successfully",
                Data = new UnreadCountResponse { UnreadCount = count }
            };
            return Ok(apiResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = ex.Message,
                Data = null
            };
            return Unauthorized(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count");
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = ex.Message,
                Data = null
            };
            return BadRequest(apiResponse);
        }
    }
}
