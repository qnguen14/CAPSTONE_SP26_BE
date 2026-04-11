using System.Linq.Expressions;
using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Message;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Metadata;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Service.Implements;

public class MessageService : BaseService<ChatMessage>, IMessageService
{
    public MessageService(
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
    {
    }

    public async Task<PaginatedResponse<MessageDTO>> GetMessagesAsync(Guid otherUserId, int page, int limit)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        if (otherUserId == Guid.Empty)
        {
            throw new ArgumentException("otherUserId is required");
        }

        page = page < 1 ? 1 : page;
        limit = limit <= 0 ? 20 : limit;
        limit = Math.Min(limit, 100);
        var skip = (page - 1) * limit;

        Expression<Func<ChatMessage, bool>> predicate =
            m => (m.SenderId == currentUserId && m.RecipientId == otherUserId) ||
                 (m.SenderId == otherUserId && m.RecipientId == currentUserId);

        var total = await _unitOfWork.GetRepository<ChatMessage>().CountAsync(predicate);

        var query = _unitOfWork.GetRepository<ChatMessage>().CreateBaseQuery(
            predicate: predicate,
            orderBy: q => q.OrderBy(m => m.SentAt),
            include: q => q
                .Include(m => m.Sender).ThenInclude(u => u.Worker)
                .Include(m => m.Sender).ThenInclude(u => u.Farmer)
                .Include(m => m.Recipient).ThenInclude(u => u.Worker)
                .Include(m => m.Recipient).ThenInclude(u => u.Farmer),
            asNoTracking: true);

        var items = await query.Skip(skip).Take(limit).ToListAsync();

        return new PaginatedResponse<MessageDTO>
        {
            Data = items.Select(m => new MessageDTO
            {
                Id = m.Id,
                SenderId = m.SenderId,
                ReceiverId = m.RecipientId,
                Content = m.MessageContent,
                Read = m.IsRead,
                CreatedAt = m.SentAt,
                Sender = BuildUserBrief(m.Sender),
                Receiver = BuildUserBrief(m.Recipient)
            }).ToList(),
            Pagination = new PaginationMetadata
            {
                Page = page,
                Limit = limit,
                Total = total,
                TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit)
            }
        };
    }

    public async Task<MessageDTO> SendMessageAsync(Guid receiverId, string content)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        if (receiverId == Guid.Empty)
        {
            throw new ArgumentException("receiverId is required");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("content is required");
        }

        var entity = new ChatMessage
        {
            Id = Guid.NewGuid(),
            SenderId = currentUserId,
            RecipientId = receiverId,
            MessageContent = content.Trim(),
            IsRead = false,
            SentAt = DateTime.UtcNow,
            ReadAt = null
        };

        await _unitOfWork.GetRepository<ChatMessage>().InsertAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        // Reload with navigation properties to populate Sender/Receiver
        var saved = await _unitOfWork.GetRepository<ChatMessage>().CreateBaseQuery(
            predicate: m => m.Id == entity.Id,
            orderBy: null,
            include: q => q
                .Include(m => m.Sender).ThenInclude(u => u.Worker)
                .Include(m => m.Sender).ThenInclude(u => u.Farmer)
                .Include(m => m.Recipient).ThenInclude(u => u.Worker)
                .Include(m => m.Recipient).ThenInclude(u => u.Farmer),
            asNoTracking: true)
            .FirstOrDefaultAsync() ?? entity;

        return new MessageDTO
        {
            Id = saved.Id,
            SenderId = saved.SenderId,
            ReceiverId = saved.RecipientId,
            Content = saved.MessageContent,
            Read = saved.IsRead,
            CreatedAt = saved.SentAt,
            Sender = BuildUserBrief(saved.Sender),
            Receiver = BuildUserBrief(saved.Recipient)
        };
    }

    public async Task<int> MarkConversationAsReadAsync(Guid senderId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        if (senderId == Guid.Empty)
        {
            throw new ArgumentException("senderId is required");
        }

        Expression<Func<ChatMessage, bool>> predicate =
            m => m.SenderId == senderId && m.RecipientId == currentUserId && !m.IsRead;

        var unreadMessagesQuery = _unitOfWork.GetRepository<ChatMessage>().CreateBaseQuery(
            predicate: predicate,
            orderBy: null,
            include: null,
            asNoTracking: false);

        var messagesList = await unreadMessagesQuery.ToListAsync();
        if (!messagesList.Any())
        {
            return 0;
        }

        var now = DateTime.UtcNow;
        foreach (var msg in messagesList)
        {
            msg.IsRead = true;
            msg.ReadAt = now;
            _unitOfWork.GetRepository<ChatMessage>().UpdateAsync(msg);
        }

        await _unitOfWork.SaveChangesAsync();

        return messagesList.Count;
    }

    private static UserBriefDTO? BuildUserBrief(User? user)
    {
        if (user is null) return null;

        var name = user.Worker?.FullName
                   ?? user.Farmer?.ContactName
                   ?? user.Email;

        var avatar = user.Worker?.AvatarUrl
                     ?? user.Farmer?.AvatarUrl;

        return new UserBriefDTO
        {
            Id = user.Id,
            Name = name,
            AvatarUrl = avatar
        };
    }
}

