using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class CreateDisputeReportCommentRequest : IValidatableObject
{
    /// <summary>
    /// Message body. Required when JobPostId is null; can be empty when attaching a job post.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    public string? AttachmentUrl { get; set; }

    /// <summary>
    /// Optional. Admin only - targets the message to a specific user (reporter or accused).
    /// When null the sender's own message is visible to admin + all parties.
    /// </summary>
    public Guid? TargetUserId { get; set; }

    /// <summary>
    /// Optional. Admin only - attaches a job post that renders as an embedded link card.
    /// </summary>
    public Guid? JobPostId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (JobPostId == null && string.IsNullOrWhiteSpace(Content))
        {
            yield return new ValidationResult(
                "Content is required when no job post is attached.",
                new[] { nameof(Content) });
        }
    }
}
