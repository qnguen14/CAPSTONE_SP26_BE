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
            ActiveJobs = activeJobsList.Select(j => new ActiveJobSummaryDTO{
                Id = j.Id,
                Title = j.Title,
                WorkersNeeded = j.WorkersNeeded,
                WorkersAccepted = j.WorkersAccepted,
                IsUrgent = j.IsUrgent,
                StatusId = j.StatusId,
                CreatedAt = j.CreatedAt
            }).ToList()
        };                                                    
    }
}