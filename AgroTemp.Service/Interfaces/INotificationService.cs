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

    Task RegisterDeviceTokenAsync(Guid userId, string token, string? deviceName = null);
    Task UnregisterDeviceTokenAsync(Guid userId, string token);
    Task<List<string>> GetActiveTokensByUserAsync(Guid userId);
    
    Task<bool> SendPushNotificationAsync(Guid userId, string title, string body, Dictionary<string, object>? data = null);
}
