using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class DisputeReportCommentDTO
{
    public Guid Id { get; set; }
    public Guid DisputeReportId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } // We might need to map from User (either Farmer, Worker or Admin)
    public UserRole Role { get; set; }
    public string Content { get; set; }
    public string? AttachmentUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
