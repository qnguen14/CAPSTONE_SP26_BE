using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.DTO.Notification;

public class NotificationDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public NotificationType Type { get; set; }
    public string TypeName => Type.ToString();
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
