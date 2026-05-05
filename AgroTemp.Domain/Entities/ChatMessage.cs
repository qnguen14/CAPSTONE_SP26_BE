using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Chat_Message")]
public class ChatMessage
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Sender))]
    [Column("sender_id")]
    public Guid SenderId { get; set; }
    public virtual User Sender { get; set; }

    [Required]
    [ForeignKey(nameof(Recipient))]
    [Column("recipient_id")]
    public Guid RecipientId { get; set; }
    public virtual User Recipient { get; set; }

    [Required]
    [Column("message_content")]
    public string MessageContent { get; set; }

    [Required]
    [Column("is_read")]
    public bool IsRead { get; set; }

    [Required]
    [Column("sent_at")]
    public DateTime SentAt { get; set; }

    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    [Column("job_post_id")]
    [ForeignKey(nameof(JobPost))]
    public Guid? JobPostId { get; set; }
    public virtual JobPost? JobPost { get; set; }
}
