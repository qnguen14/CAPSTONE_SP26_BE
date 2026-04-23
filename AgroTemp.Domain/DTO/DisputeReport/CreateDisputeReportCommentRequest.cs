using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class CreateDisputeReportCommentRequest
{
    [Required]
    public string Content { get; set; }

    public string? AttachmentUrl { get; set; }

    /// <summary>
    /// Optional. Admin only — targets the message to a specific user (reporter or accused).
    /// When null the sender's own message is visible to admin + all parties.
    /// </summary>
    public Guid? TargetUserId { get; set; }
}
