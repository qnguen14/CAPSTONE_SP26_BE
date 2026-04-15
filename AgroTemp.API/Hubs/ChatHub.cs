using System.Collections.Concurrent;
using System.Security.Claims;
using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Message;
using AgroTemp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly AgroTempDbContext _dbContext;
    private readonly ILogger<ChatHub> _logger;
    private static readonly ConcurrentDictionary<string, Guid> ActiveConnections = new();

    public ChatHub(AgroTempDbContext dbContext, ILogger<ChatHub> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        // Put each authenticated user into a group by their userId.
        var userId = GetCurrentUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());

        ActiveConnections[Context.ConnectionId] = userId;

        _logger.LogInformation(
            "SignalR connected. ConnectionId={ConnectionId}, UserId={UserId}, TotalConnections={TotalConnections}, UniqueUsers={UniqueUsers}",
            Context.ConnectionId,
            userId,
            ActiveConnections.Count,
            ActiveConnections.Values.Distinct().Count()
        );

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        ActiveConnections.TryRemove(Context.ConnectionId, out var userId);

        _logger.LogInformation(
            "SignalR disconnected. ConnectionId={ConnectionId}, UserId={UserId}, TotalConnections={TotalConnections}, UniqueUsers={UniqueUsers}",
            Context.ConnectionId,
            userId,
            ActiveConnections.Count,
            ActiveConnections.Values.Distinct().Count()
        );

        await base.OnDisconnectedAsync(exception);
    }

    public Task<object> GetConnectionStats()
    {
        var currentUserId = GetCurrentUserId();
        var myConnections = ActiveConnections.Values.Count(uid => uid == currentUserId);

        return Task.FromResult<object>(new
        {
            totalConnections = ActiveConnections.Count,
            uniqueUsers = ActiveConnections.Values.Distinct().Count(),
            myConnections
        });
    }

    public async Task SendMessage(Guid receiverId, string content)
    {
        var senderId = GetCurrentUserId();

        if (receiverId == Guid.Empty)
        {
            throw new HubException("receiverId is required");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new HubException("content is required");
        }

        var messageEntity = new ChatMessage
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            RecipientId = receiverId,
            MessageContent = content.Trim(),
            IsRead = false,
            SentAt = DateTime.UtcNow,
            ReadAt = null
        };

        _dbContext.ChatMessages.Add(messageEntity);
        await _dbContext.SaveChangesAsync();

        var messageDto = new MessageDTO
        {
            Id = messageEntity.Id,
            SenderId = messageEntity.SenderId,
            ReceiverId = messageEntity.RecipientId,
            Content = messageEntity.MessageContent,
            Read = messageEntity.IsRead,
            CreatedAt = messageEntity.SentAt
        };

        // Notify both sender and receiver in realtime.
        await Clients.Group(senderId.ToString()).SendAsync("NewMessage", messageDto);
        await Clients.Group(receiverId.ToString()).SendAsync("NewMessage", messageDto);
    }

    private Guid GetCurrentUserId()
    {
        var idClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
        {
            throw new HubException("User not authenticated");
        }

        return userId;
    }
}

