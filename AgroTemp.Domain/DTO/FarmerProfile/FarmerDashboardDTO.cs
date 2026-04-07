using AgroTemp.Domain.DTO.Job.JobPost;

namespace AgroTemp.Domain.DTO.FarmerProfile;

public class FarmerDashboardDTO
{
    public FarmerProfileSummaryDTO Profile { get; set; }
    public WalletSummaryDTO Wallet { get; set; }
    public DashboardCountersDTO Counters { get; set; }
    public List<ActiveJobSummaryDTO> ActiveJobs { get; set; }
    public List<WeeklyActivityDTO> WeeklyActivity { get; set; }
    public List<JobStatusDistributionDTO> JobStatusDistribution { get; set; }
    public List<ScheduleDateDTO> SchedulesDates { get; set; }
}

public class FarmerProfileSummaryDTO
{
    public string ContactName { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalJobsPosted { get; set; }
    public int TotalJobsCompleted { get; set; }
    public string AvatarUrl { get; set; }
}

public class WalletSummaryDTO
{
    public decimal AvailableBalance { get; set; }
    public decimal LockedBalance { get; set; }
}

public class DashboardCountersDTO
{
    public int PendingApplications { get; set; }
    public int WorkReportsToApprove { get; set; }
    public int TotalWorkersCurrentlyHired { get; set; }
}

public class ActiveJobSummaryDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int WorkersNeeded { get; set; }
    public int WorkersAccepted { get; set; }
    public bool IsUrgent { get; set; }
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WeeklyActivityDTO
{
    public string Name { get; set; }
    public int ApplicationsCount { get; set; }
    public int JobPostsCount { get; set; }
}

public class JobStatusDistributionDTO
{
    public int StatusId { get; set; }
    public string StatusName { get; set; }
    public int Count { get; set; }
}

public class ScheduleDateDTO
{
    public string ScheduleDate { get; set; }
}
