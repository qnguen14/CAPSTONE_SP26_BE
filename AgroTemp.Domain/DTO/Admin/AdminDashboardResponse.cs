namespace AgroTemp.Domain.DTO.Admin;

public class AdminDashboardResponse
{
    public AdminDashboardStatsDto Stats { get; set; } = new();
    public List<AdminDashboardTrendDto> Trends { get; set; } = new();
    public AdminDashboardJobStatusBreakdownDto JobStatusBreakdown { get; set; } = new();
    public List<AdminDashboardRecentActivityDto> RecentActivities { get; set; } = new();
}

public class AdminDashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveJobs { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal CompletionRate { get; set; }
}

public class AdminDashboardTrendDto
{
    public string Month { get; set; } = string.Empty;
    public int NewUsers { get; set; }
    public int NewJobs { get; set; }
    public decimal Revenue { get; set; }
}

public class AdminDashboardJobStatusBreakdownDto
{
    public int Completed { get; set; }
    public int InProgress { get; set; }
    public int Pending { get; set; }
    public int Cancelled { get; set; }
}

public class AdminDashboardRecentActivityDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
