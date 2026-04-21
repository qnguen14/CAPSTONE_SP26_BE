namespace AgroTemp.Domain.DTO.WorkerProfile;

public class WorkerApplicationStatsDTO
{
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int AcceptedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public int CancelledApplications { get; set; }
    public int CompletedJobs { get; set; }
    public decimal TotalEarnings { get; set; }
}
