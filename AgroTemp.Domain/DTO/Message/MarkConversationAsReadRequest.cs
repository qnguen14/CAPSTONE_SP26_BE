namespace AgroTemp.Domain.DTO.Message;

public class MarkConversationAsReadRequest
{
    // Mark messages sent from this user to the current user as read.
    public Guid SenderId { get; set; }
}

