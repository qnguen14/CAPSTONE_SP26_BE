using AgroTemp.Domain.DTO.Notification;

namespace AgroTemp.Service.Interfaces;

public interface INotificationService
{
    Task<NotificationDTO> CreateAsync(CreateNotificationRequest request);
    Task<List<NotificationDTO>> GetByUserAsync(Guid userId);
    Task<List<NotificationDTO>> GetUnreadByUserAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteAsync(Guid notificationId);
}
