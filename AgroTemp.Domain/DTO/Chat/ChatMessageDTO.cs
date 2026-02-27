namespace AgroTemp.Domain.DTO.Chat;

public class ChatMessageDTO
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public Guid RecipientId { get; set; }
    public string RecipientName { get; set; }
    public string MessageContent { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
