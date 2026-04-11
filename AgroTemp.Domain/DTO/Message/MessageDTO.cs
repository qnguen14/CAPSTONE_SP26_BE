using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.DTO.Message;

public class UserBriefDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

public class MessageDTO
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }
    public User Sender { get; set; }

    public Guid ReceiverId { get; set; }
    public User Receiver { get; set; }

    public string Content { get; set; } = string.Empty;

    public bool Read { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserBriefDTO? Sender { get; set; }

    public UserBriefDTO? Receiver { get; set; }
}
