using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class UpdateDisputeReportRequest
{
    [Range(1, int.MaxValue)]
    public int? DisputeTypeId { get; set; }

    [StringLength(512)]
    public string? Reason { get; set; }

    public string? Description { get; set; }
    public string? EvidenceUrl { get; set; }
    public int? StatusId { get; set; }
    public string? AdminNote { get; set; }
    public Guid? ResolvedById { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
