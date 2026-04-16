using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class CreateDisputeReportCommentRequest
{
    [Required]
    public string Content { get; set; }

    public string? AttachmentUrl { get; set; }
}
