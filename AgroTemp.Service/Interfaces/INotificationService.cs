using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.Metadata;

namespace AgroTemp.Service.Interfaces;

public interface INotificationService
{
    Task<NotificationDTO> CreateAsync(CreateNotificationRequest request);
    Task<List<NotificationDTO>> GetByUserAsync(Guid userId);
    Task<PaginatedResponse<NotificationDTO>> GetPaginatedByUserAsync(Guid userId, NotificationFilterRequest filter);
    Task<List<NotificationDTO>> GetUnreadByUserAsync(Guid userId);
    Task<NotificationDTO> MarkAsReadAsync(Guid notificationId);
    Task<List<NotificationDTO>> MarkAllAsReadAsync(Guid userId);
    Task DeleteAsync(Guid notificationId);

    Task RegisterDeviceTokenAsync(Guid userId, string token, string? deviceName = null);
    Task UnregisterDeviceTokenAsync(Guid userId, string token);
    Task<List<string>> GetActiveTokensByUserAsync(Guid userId);
    
    Task<bool> SendPushNotificationAsync(Guid userId, string title, string body, Dictionary<string, object>? data = null);
}
