using AgroTemp.Domain.DTO.Chat;

namespace AgroTemp.Service.Interfaces;

public interface IChatService
{
    Task<ChatMessageDTO> SendMessageAsync(SendMessageRequest request);
    Task<List<ChatMessageDTO>> GetConversationAsync(Guid otherUserId);
    Task<List<ConversationPreviewDTO>> GetConversationsAsync();
    Task MarkAsReadAsync(Guid messageId);
    Task<int> GetUnreadCountAsync();
}
