namespace AgroTemp.Domain.DTO.Notification;

public class SendPushNotificationRequest
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}