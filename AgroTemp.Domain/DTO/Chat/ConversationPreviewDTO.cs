namespace AgroTemp.Domain.DTO.Chat;

public class ConversationPreviewDTO
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserRole { get; set; }
    public string LastMessage { get; set; }
    public DateTime LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
    public bool IsLastMessageFromMe { get; set; }
}
