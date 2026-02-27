using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Chat;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Service.Implements;

public class ChatService : BaseService<ChatMessage>, IChatService
{
    public ChatService(
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
    {
    }

    public async Task<ChatMessageDTO> SendMessageAsync(SendMessageRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            // Verify recipient exists and is active
            var recipient = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(
                    predicate: u => u.Id == request.RecipientId && u.IsActive,
                    include: query => query
                        .Include(u => u.WorkerProfile)
                        .Include(u => u.FarmerProfile));

            if (recipient == null)
            {
                throw new Exception("Recipient not found or inactive");
            }

            // Get sender info for response
            var sender = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(
                    predicate: u => u.Id == currentUserId,
                    include: query => query
                        .Include(u => u.WorkerProfile)
                        .Include(u => u.FarmerProfile));

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                SenderId = currentUserId,
                RecipientId = request.RecipientId,
                MessageContent = request.MessageContent,
                IsRead = false,
                SentAt = DateTime.UtcNow,
                ReadAt = null
            };

            await _unitOfWork.GetRepository<ChatMessage>().InsertAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return new ChatMessageDTO
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = GetUserDisplayName(sender),
                RecipientId = message.RecipientId,
                RecipientName = GetUserDisplayName(recipient),
                MessageContent = message.MessageContent,
                IsRead = message.IsRead,
                SentAt = message.SentAt,
                ReadAt = message.ReadAt
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending message: {ex.Message}");
        }
    }

    public async Task<List<ChatMessageDTO>> GetConversationAsync(Guid otherUserId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var messages = await _unitOfWork.GetRepository<ChatMessage>()
                .GetListAsync(
                    predicate: m => (m.SenderId == currentUserId && m.RecipientId == otherUserId) ||
                                   (m.SenderId == otherUserId && m.RecipientId == currentUserId),
                    orderBy: q => q.OrderBy(m => m.SentAt),
                    include: query => query
                        .Include(m => m.Sender)
                            .ThenInclude(u => u.WorkerProfile)
                        .Include(m => m.Sender)
                            .ThenInclude(u => u.FarmerProfile)
                        .Include(m => m.Recipient)
                            .ThenInclude(u => u.WorkerProfile)
                        .Include(m => m.Recipient)
                            .ThenInclude(u => u.FarmerProfile));

            return messages.Select(m => new ChatMessageDTO
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = GetUserDisplayName(m.Sender),
                RecipientId = m.RecipientId,
                RecipientName = GetUserDisplayName(m.Recipient),
                MessageContent = m.MessageContent,
                IsRead = m.IsRead,
                SentAt = m.SentAt,
                ReadAt = m.ReadAt
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving conversation: {ex.Message}");
        }
    }

    public async Task<List<ConversationPreviewDTO>> GetConversationsAsync()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            // Get all messages where user is sender or recipient
            var allMessages = await _unitOfWork.GetRepository<ChatMessage>()
                .GetListAsync(
                    predicate: m => m.SenderId == currentUserId || m.RecipientId == currentUserId,
                    orderBy: q => q.OrderByDescending(m => m.SentAt),
                    include: query => query
                        .Include(m => m.Sender)
                            .ThenInclude(u => u.WorkerProfile)
                        .Include(m => m.Sender)
                            .ThenInclude(u => u.FarmerProfile)
                        .Include(m => m.Recipient)
                            .ThenInclude(u => u.WorkerProfile)
                        .Include(m => m.Recipient)
                            .ThenInclude(u => u.FarmerProfile));

            // Group by conversation partner
            var conversations = allMessages
                .GroupBy(m => m.SenderId == currentUserId ? m.RecipientId : m.SenderId)
                .Select(g =>
                {
                    var lastMessage = g.First(); // Already ordered by SentAt descending
                    var otherUser = lastMessage.SenderId == currentUserId ? lastMessage.Recipient : lastMessage.Sender;
                    var unreadCount = g.Count(m => m.RecipientId == currentUserId && !m.IsRead);

                    return new ConversationPreviewDTO
                    {
                        UserId = otherUser.Id,
                        UserName = GetUserDisplayName(otherUser),
                        UserRole = ((UserRole)otherUser.RoleId).ToString(),
                        LastMessage = lastMessage.MessageContent,
                        LastMessageTime = lastMessage.SentAt,
                        UnreadCount = unreadCount,
                        IsLastMessageFromMe = lastMessage.SenderId == currentUserId
                    };
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();

            return conversations;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving conversations: {ex.Message}");
        }
    }

    public async Task MarkAsReadAsync(Guid messageId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var message = await _unitOfWork.GetRepository<ChatMessage>()
                .FirstOrDefaultAsync(predicate: m => m.Id == messageId);

            if (message == null)
            {
                throw new Exception("Message not found");
            }

            // Only recipient can mark as read
            if (message.RecipientId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only mark your own messages as read");
            }

            if (!message.IsRead)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<ChatMessage>().UpdateAsync(message);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error marking message as read: {ex.Message}");
        }
    }

    public async Task<int> GetUnreadCountAsync()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var count = await _unitOfWork.GetRepository<ChatMessage>()
                .CountAsync(predicate: m => m.RecipientId == currentUserId && !m.IsRead);

            return count;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting unread count: {ex.Message}");
        }
    }

    private string GetUserDisplayName(User user)
    {
        if (user == null) return "Unknown User";

        if (user.Role == UserRole.Worker && user.WorkerProfile != null)
        {
            return user.WorkerProfile.FullName ?? user.Email;
        }
        else if (user.Role == UserRole.Farmer && user.FarmerProfile != null)
        {
            return user.FarmerProfile.ContactName ?? user.FarmerProfile.OrganizationName ?? user.Email;
        }

        return user.Email;
    }
}
