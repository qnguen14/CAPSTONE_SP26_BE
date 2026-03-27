namespace AgroTemp.Domain.DTO.Message;

public class CreateMessageRequest
{
    public Guid ReceiverId { get; set; }

    public string Content { get; set; } = string.Empty;
}

