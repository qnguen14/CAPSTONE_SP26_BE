using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.FarmerProfile;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Service.Implements;

public class DashboardService : BaseService<JobPost>, IDashboardService
{
    public DashboardService(IUnitOfWork<AgroTempDbContext> unitOfWork,
                            IHttpContextAccessor httpContextAccessor,
                            IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
    {
    }

    public async Task<FarmerDashboardDTO> GetFarmerDashboardAsync()
    {
        var currentUserId = GetCurrentUserId();
        
        var farmer = await _unitOfWork.GetRepository<Farmer>()
                                        .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId);

        if(farmer == null)
        {
            throw new UnauthorizedAccessException("Farmer profile not found.");
        }

        var wallet = await _unitOfWork.GetRepository<Wallet>()
                                        .FirstOrDefaultAsync(predicate: w => w.UserId == currentUserId);

        var activeJobsList = await _unitOfWork.GetRepository<JobPost>()
                                            .GetListAsync(predicate: j => j.FarmerId == farmer.Id 
                                                                && (j.StatusId == (int)JobPostStatus.Published
                                                                || j.StatusId == (int)JobPostStatus.InProgress),
                                                        orderBy: q => q.OrderByDescending(j => j.CreatedAt));

        var pendingAppsCount = await _unitOfWork.GetRepository<JobApplication>()
                                                .CountAsync(ja => ja.JobPost.FarmerId == farmer.Id 
                                                                && ja.StatusId == (int)ApplicationStatus.Pending);

        var reportsToApproveCount = await _unitOfWork.GetRepository<JobDetail>()
                                                    .CountAsync(jd => jd.JobPost.FarmerId == farmer.Id
                                                                    && jd.StatusId == (int)JobStatus.Reported);

        // Weekly Activity
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-6);

        var weeklyJobPosts = await _unitOfWork.GetRepository<JobPost>()
                                             .GetListAsync(predicate: j => j.FarmerId == farmer.Id
                                                                        && j.CreatedAt.Date >= weekStart
                                                                        && j.CreatedAt.Date <= today);

        var weeklyApplications = await _unitOfWork.GetRepository<JobApplication>()
                                                  .GetListAsync(predicate: ja => ja.JobPost.FarmerId == farmer.Id
                                                                               && ja.AppliedAt.Date >= weekStart
                                                                               && ja.AppliedAt.Date <= today);

        var weeklyActivity = Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var date = weekStart.AddDays(offset);
                return new WeeklyActivityDTO
                {
                    Name = date.ToString("ddd, MMM dd"),
                    ApplicationsCount = weeklyApplications.Count(ja => ja.AppliedAt.Date == date),
                    JobPostsCount = weeklyJobPosts.Count(j => j.CreatedAt.Date == date)
                };
            })
            .ToList();

        // Job Status Distribution
        var allFarmerJobPosts = await _unitOfWork.GetRepository<JobPost>()
                                                 .GetListAsync(predicate: j => j.FarmerId == farmer.Id);

        var jobStatusDistribution = allFarmerJobPosts
            .GroupBy(j => j.StatusId)
            .Select(g => new JobStatusDistributionDTO
            {
                StatusId = g.Key,
                StatusName = ((JobPostStatus)g.Key).ToString(),
                Count = g.Count()
            })
            .ToList();

        // Scheduled Dates
        var scheduledDetails = await _unitOfWork.GetRepository<JobDetail>()
                                                .GetListAsync(predicate: jd => jd.JobPost.FarmerId == farmer.Id
                                                                            && jd.WorkDate.HasValue
                                                                            && (jd.StatusId == (int)JobStatus.InProgress
                                                                             || jd.StatusId == (int)JobStatus.Reported));

        var scheduleDates = scheduledDetails
            .Where(jd => jd.WorkDate.HasValue)
            .Select(jd => jd.WorkDate!.Value.Date)
            .Distinct()
            .OrderBy(d => d)
            .Select(d => new ScheduleDateDTO { ScheduleDate = d.ToString("yyyy-MM-dd") })
            .ToList();

        return new FarmerDashboardDTO
        {
            Profile = new FarmerProfileSummaryDTO
            {
                ContactName = farmer.ContactName,
                AverageRating = farmer.AverageRating,
                TotalJobsPosted = farmer.TotalJobsPosted,
                TotalJobsCompleted = farmer.TotalJobsCompleted,
                AvatarUrl = farmer.AvatarUrl,  
            },
            Wallet = new WalletSummaryDTO
            {
                AvailableBalance = wallet?.Balance ?? 0,
                LockedBalance = wallet?.LockedBalance ?? 0
            },
            Counters = new DashboardCountersDTO
            {
                PendingApplications = pendingAppsCount,
                WorkReportsToApprove = reportsToApproveCount,
                TotalWorkersCurrentlyHired = activeJobsList.Sum(j => j.WorkersAccepted)
            },
            ActiveJobs = activeJobsList.Select(j => new ActiveJobSummaryDTO
            {
                Id = j.Id,
                Title = j.Title,
                WorkersNeeded = j.WorkersNeeded,
                WorkersAccepted = j.WorkersAccepted,
                IsUrgent = j.IsUrgent,
                StatusId = j.StatusId,
                CreatedAt = j.CreatedAt
            }).ToList(),
            WeeklyActivity = weeklyActivity,
            JobStatusDistribution = jobStatusDistribution,
            SchedulesDates = scheduleDates
        };                                                    
    }
}