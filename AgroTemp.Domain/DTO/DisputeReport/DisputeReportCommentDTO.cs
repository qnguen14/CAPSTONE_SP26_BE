using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class DisputeReportCommentDTO
{
    public Guid Id { get; set; }
    public Guid DisputeReportId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public UserRole Role { get; set; }
    public string Content { get; set; }
    public string? AttachmentUrl { get; set; }
    /// <summary>The user this admin message is addressed to. Null = sent by reporter/accused (not a targeted admin message).</summary>
    public Guid? TargetUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
