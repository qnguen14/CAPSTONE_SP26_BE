using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Chat;

public class SendMessageRequest
{
    [Required]
    public Guid RecipientId { get; set; }
    
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string MessageContent { get; set; }
}
