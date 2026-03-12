using System.ComponentModel.DataAnnotations;
using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.DTO.Notification;

public class CreateNotificationRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public NotificationType Type { get; set; }

    [Required]
    [StringLength(256)]
    public string Title { get; set; }

    [Required]
    public string Message { get; set; }

    public Guid? RelatedEntityId { get; set; }
}
