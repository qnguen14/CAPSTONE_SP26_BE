using AgroTemp.Domain.DTO.Notification;

namespace AgroTemp.Service.Interfaces;

public interface IExpoPushService
{
    Task<bool> SendPushNotificationAsync(Guid userId, string title, string body, Dictionary<string, object>? data = null);
    Task<bool> SendPushToTokenAsync(string expoPushToken, string title, string body, Dictionary<string, object>? data = null);
    Task<List<string>> SendPushToMultipleAsync(List<string> tokens, string title, string body, Dictionary<string, object>? data = null);
}