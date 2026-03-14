using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum NotificationType
{
    JobAcceptance = 1,
    Reminder,
    PaymentConfirmation,
    NearbyJobOpening
}

[Table("Notification")]
public class Notification
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    [Column("user_id")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public Guid? RelatedEntityId { get; set; }
    [Required] 
    [Column("type")]
    public NotificationType Type { get; set; }

    [Required]
    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; }

    [Required]
    [Column("message")]
    public string Message { get; set; }

    [Required]
    [Column("is_read")]
    public bool IsRead { get; set; }

    [Required]
    [Column("sent_at")]
    public DateTime SentAt { get; set; }

    [Column("read_at")]
    public DateTime? ReadAt { get; set; }
}
