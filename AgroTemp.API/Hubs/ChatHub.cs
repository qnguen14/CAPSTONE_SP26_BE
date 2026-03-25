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

    public ChatHub(AgroTempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task OnConnectedAsync()
    {
        // Put each authenticated user into a group by their userId.
        var userId = GetCurrentUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
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

