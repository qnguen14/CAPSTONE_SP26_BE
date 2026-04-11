using AgroTemp.Domain.DTO.Message;
using AgroTemp.Domain.Metadata;

namespace AgroTemp.Service.Interfaces;

public interface IMessageService
{
    Task<PaginatedResponse<MessageDTO>> GetMessagesAsync(Guid otherUserId, int page, int limit);

    Task<MessageDTO> SendMessageAsync(Guid receiverId, string content);

    // Mark all unread messages sent from `senderId` to the current user as read.
    Task<int> MarkConversationAsReadAsync(Guid senderId);

    Task<List<ConversationDTO>> GetRecentConversationsAsync();

}

