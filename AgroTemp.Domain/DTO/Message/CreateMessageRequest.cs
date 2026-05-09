namespace AgroTemp.Domain.DTO.Message;

public class CreateMessageRequest
{
    public Guid ReceiverId { get; set; }

    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Optional. Admin only - attaches a job post that renders as an embedded link card.
    /// </summary>
    public Guid? JobPostId { get; set; }
}

