using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Notification;

public class MarkNotificationReadRequest
{
    [Required]
    public Guid NotificationId { get; set; }
}
