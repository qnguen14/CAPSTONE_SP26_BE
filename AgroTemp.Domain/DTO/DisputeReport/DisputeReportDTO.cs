using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class CustomDisputeReportDTO
{
    public List<DisputeReportDTO>? DisputeReports { get; set; }
    public List<WorkerProfileDTO> Workers { get; set; }
    public List<FarmerProfileDTO> Farmers { get; set; }
}

public class DisputeReportDTO
{
    public Guid Id { get; set; }
    public Guid? FarmerId { get; set; }
    public Guid? WorkerId { get; set; }
    public Guid JobPostId { get; set; }
    public JobPostDTO JobPost { get; set; }
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

    public Guid? ReporterUserId { get; set; }

    public Guid? AccusedUserId { get; set; }

    public int PenaltyTargetId { get; set; }
    public PenaltyTarget PenaltyTarget => (PenaltyTarget)PenaltyTargetId;
}
