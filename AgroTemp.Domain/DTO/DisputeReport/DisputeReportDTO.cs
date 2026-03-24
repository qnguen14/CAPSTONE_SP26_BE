using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class DisputeReportDTO
{
    public Guid Id { get; set; }
    public Guid? FarmerId { get; set; }
    public Guid? WorkerId { get; set; }
    public Guid JobPostId { get; set; }
    public int DisputeTypeId { get; set; }
    public DisputeType DisputeType => (DisputeType)DisputeTypeId;
    public string Reason { get; set; }
    public string? Description { get; set; }
    public string? EvidenceUrl { get; set; }
    public int StatusId { get; set; }
    public DisputeStatus Status => (DisputeStatus)StatusId;
    public string? AdminNote { get; set; }
    public Guid? ResolvedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
