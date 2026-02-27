using AgroTemp.Domain.DTO.Chat;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AgroTemp.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation($"User {userId} connected to chat hub");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation($"User {userId} disconnected from chat hub");
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Send a message to another user in real-time
    /// </summary>
    public async Task SendMessage(Guid recipientId, string messageContent)
    {
        try
        {
            var request = new SendMessageRequest
            {
                RecipientId = recipientId,
                MessageContent = messageContent
            };

            var message = await _chatService.SendMessageAsync(request);

            // Send to recipient
            await Clients.User(recipientId.ToString()).SendAsync("ReceiveMessage", message);

            // Confirm to sender
            await Clients.Caller.SendAsync("MessageSent", message);

            _logger.LogInformation($"Message sent from {message.SenderId} to {recipientId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message via SignalR");
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    /// <summary>
    /// Mark a message as read
    /// </summary>
    public async Task MarkAsRead(Guid messageId)
    {
        try
        {
            await _chatService.MarkAsReadAsync(messageId);

            await Clients.Caller.SendAsync("MessageRead", messageId);

            _logger.LogInformation($"Message {messageId} marked as read");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as read via SignalR");
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    /// <summary>
    /// User is typing indicator
    /// </summary>
    public async Task UserTyping(Guid recipientId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Clients.User(recipientId.ToString()).SendAsync("UserTyping", userId);
    }

    /// <summary>
    /// User stopped typing indicator
    /// </summary>
    public async Task UserStoppedTyping(Guid recipientId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Clients.User(recipientId.ToString()).SendAsync("UserStoppedTyping", userId);
    }
}
