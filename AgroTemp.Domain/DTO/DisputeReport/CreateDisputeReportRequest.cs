using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class CreateDisputeReportRequest
{
    public Guid? FarmerId { get; set; }
    public Guid? WorkerId { get; set; }

    [Required]
    public Guid JobPostId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int DisputeTypeId { get; set; }

    [Required]
    [StringLength(512)]
    public string Reason { get; set; }

    public string? Description { get; set; }
    public string? EvidenceUrl { get; set; }
}
