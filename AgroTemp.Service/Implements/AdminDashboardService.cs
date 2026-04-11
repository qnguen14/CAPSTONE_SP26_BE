using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Admin;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Service.Implements;

public class AdminDashboardService : BaseService<User>, IAdminDashboardService
{
    private sealed class PaymentRevenuePoint
    {
        public decimal Amount { get; set; }
        public DateTime PaidOrCreatedAt { get; set; }
    }

    public AdminDashboardService(
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
    {
    }

    public async Task<AdminDashboardResponse> GetDashboardAsync(int trendMonths = 6, int activityLimit = 20)
    {
        var now = DateTime.UtcNow;
        var normalizedMonths = Math.Clamp(trendMonths, 1, 24);
        var normalizedActivityLimit = Math.Clamp(activityLimit, 1, 100);

        var totalUsers = await _unitOfWork.GetRepository<User>().CountAsync();

        var activeJobs = await _unitOfWork.GetRepository<JobPost>().CountAsync(x =>
            x.StatusId == (int)JobPostStatus.Published ||
            x.StatusId == (int)JobPostStatus.InProgress);

        var completedJobs = await _unitOfWork.GetRepository<JobPost>().CountAsync(x => x.StatusId == (int)JobPostStatus.Completed);
        var inProgressJobs = await _unitOfWork.GetRepository<JobPost>().CountAsync(x => x.StatusId == (int)JobPostStatus.InProgress);
        var cancelledJobs = await _unitOfWork.GetRepository<JobPost>().CountAsync(x => x.StatusId == (int)JobPostStatus.Cancelled);
        var pendingJobs = await _unitOfWork.GetRepository<JobPost>().CountAsync(x =>
            x.StatusId == (int)JobPostStatus.Draft ||
            x.StatusId == (int)JobPostStatus.Published ||
            x.StatusId == (int)JobPostStatus.Closed);

        var totalJobs = completedJobs + inProgressJobs + pendingJobs + cancelledJobs;
        var completionRate = totalJobs == 0 ? 0m : Math.Round((decimal)completedJobs * 100m / totalJobs, 1);

        var paidPayments = await _unitOfWork.Context.Set<Payment>()
            .AsNoTracking()
            .Where(x => x.Status == "PAID")
            .Select(x => new PaymentRevenuePoint
            {
                Amount = x.Amount,
                PaidOrCreatedAt = x.PaidAt ?? x.CreatedAt
            })
            .ToListAsync();

        var totalRevenue = paidPayments.Sum(x => x.Amount);
        var trends = await BuildTrendsAsync(normalizedMonths, paidPayments, now);
        var recentActivities = await BuildRecentActivitiesAsync(normalizedActivityLimit);

        return new AdminDashboardResponse
        {
            Stats = new AdminDashboardStatsDto
            {
                TotalUsers = totalUsers,
                ActiveJobs = activeJobs,
                TotalRevenue = Math.Round(totalRevenue, 2),
                CompletionRate = completionRate
            },
            Trends = trends,
            JobStatusBreakdown = new AdminDashboardJobStatusBreakdownDto
            {
                Completed = completedJobs,
                InProgress = inProgressJobs,
                Pending = pendingJobs,
                Cancelled = cancelledJobs
            },
            RecentActivities = recentActivities
        };
    }

    private async Task<List<AdminDashboardTrendDto>> BuildTrendsAsync(
        int months,
        List<PaymentRevenuePoint> paidPayments,
        DateTime now)
    {
        var startMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddMonths(-(months - 1));

        var users = await _unitOfWork.Context.Set<User>()
            .AsNoTracking()
            .Where(x => x.CreatedAt >= startMonth)
            .Select(x => x.CreatedAt)
            .ToListAsync();

        var jobs = await _unitOfWork.Context.Set<JobPost>()
            .AsNoTracking()
            .Where(x => x.CreatedAt >= startMonth)
            .Select(x => x.CreatedAt)
            .ToListAsync();

        var result = new List<AdminDashboardTrendDto>();

        for (var i = 0; i < months; i++)
        {
            var monthStart = startMonth.AddMonths(i);
            var nextMonth = monthStart.AddMonths(1);

            var monthUsers = users.Count(x => x >= monthStart && x < nextMonth);
            var monthJobs = jobs.Count(x => x >= monthStart && x < nextMonth);
            var monthRevenue = paidPayments
                .Where(x => x.PaidOrCreatedAt >= monthStart && x.PaidOrCreatedAt < nextMonth)
                .Sum(x => (decimal)x.Amount);

            result.Add(new AdminDashboardTrendDto
            {
                Month = monthStart.ToString("yyyy-MM"),
                NewUsers = monthUsers,
                NewJobs = monthJobs,
                Revenue = Math.Round(monthRevenue, 2)
            });
        }

        return result;
    }

    private async Task<List<AdminDashboardRecentActivityDto>> BuildRecentActivitiesAsync(int activityLimit)
    {
        var newUsers = await _unitOfWork.Context.Set<User>()
            .AsNoTracking()
            .Include(x => x.Worker)
            .Include(x => x.Farmer)
            .OrderByDescending(x => x.CreatedAt)
            .Take(activityLimit)
            .Select(x => new AdminDashboardRecentActivityDto
            {
                Id = x.Id,
                Type = "NEW_USER",
                Description = "Người dùng mới đăng ký",
                ActorName = x.Worker != null ? x.Worker.FullName : (x.Farmer != null ? x.Farmer.ContactName : x.Email),
                Amount = null,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        var completedJobs = await _unitOfWork.Context.Set<JobDetail>()
            .AsNoTracking()
            .Include(x => x.Worker)
            .Where(x => x.StatusId == (int)JobStatus.Completed)
            .OrderByDescending(x => x.CompletedAt ?? x.UpdatedAt ?? x.CreatedAt)
            .Take(activityLimit)
            .Select(x => new AdminDashboardRecentActivityDto
            {
                Id = x.Id,
                Type = "JOB_COMPLETED",
                Description = "Công việc đã hoàn thành",
                ActorName = x.Worker.FullName,
                Amount = x.WorkerPaymentAmount ?? x.JobPrice,
                CreatedAt = x.CompletedAt ?? x.UpdatedAt ?? x.CreatedAt
            })
            .ToListAsync();

        var resolvedDisputes = await _unitOfWork.Context.Set<DisputeReport>()
            .AsNoTracking()
            .Include(x => x.ResolvedBy)
            .Where(x => x.StatusId == (int)DisputeStatus.Resolved && x.ResolvedAt.HasValue)
            .OrderByDescending(x => x.ResolvedAt)
            .Take(activityLimit)
            .Select(x => new AdminDashboardRecentActivityDto
            {
                Id = x.Id,
                Type = "DISPUTE_RESOLVED",
                Description = "Tranh chấp đã được giải quyết",
                ActorName = x.ResolvedBy != null ? x.ResolvedBy.Email : "System",
                Amount = null,
                CreatedAt = x.ResolvedAt ?? x.CreatedAt
            })
            .ToListAsync();

            var walletTransactions = await _unitOfWork.Context.Set<WalletTransaction>()
            .AsNoTracking()
            .Include(x => x.Wallet)
                .ThenInclude(x => x.User)
                    .ThenInclude(x => x.Worker)
            .Include(x => x.Wallet)
                .ThenInclude(x => x.User)
                    .ThenInclude(x => x.Farmer)
            .Where(x =>
                x.Type == TransactionType.DEPOSIT ||
                x.Type == TransactionType.WITHDRAW ||
                x.Type == TransactionType.JOB_PAYMENT ||
                x.Type == TransactionType.REFUND ||
                x.Type == TransactionType.JOB_LOCK)
            .OrderByDescending(x => x.CreatedAt)
            .Take(activityLimit)
            .Select(x => new AdminDashboardRecentActivityDto
            {
                Id = x.Id,
                Type = x.Type == TransactionType.DEPOSIT ? "DEPOSIT" :
                    x.Type == TransactionType.WITHDRAW ? "WITHDRAW" :
                    x.Type == TransactionType.JOB_PAYMENT ? "JOB_PAYMENT" :
                    x.Type == TransactionType.REFUND ? "REFUND" :
                    "JOB_LOCK",
                Description = x.Type == TransactionType.DEPOSIT ? "Nạp tiền vào ví" :
                    x.Type == TransactionType.WITHDRAW ? "Rút tiền ra khỏi ví" :
                    x.Type == TransactionType.JOB_PAYMENT ? "Thanh toán công việc" :
                    x.Type == TransactionType.REFUND ? "Hoàn tiền vào ví" :
                    "Khóa tiền cho công việc",
                ActorName = x.Wallet.User.Worker != null
                    ? x.Wallet.User.Worker.FullName
                    : (x.Wallet.User.Farmer != null ? x.Wallet.User.Farmer.ContactName : x.Wallet.User.Email),
                Amount = x.Amount,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        var lockedAccounts = await _unitOfWork.Context.Set<User>()
            .AsNoTracking()
            .Include(x => x.Worker)
            .Include(x => x.Farmer)
            .Where(x => !x.IsActive)
            .OrderByDescending(x => x.LastWarnedAt ?? x.CreatedAt)
            .Take(activityLimit)
            .Select(x => new AdminDashboardRecentActivityDto
            {
                Id = x.Id,
                Type = "ACCOUNT_LOCKED",
                Description = "Tài khoản đã bị khóa",
                ActorName = x.Worker != null ? x.Worker.FullName : (x.Farmer != null ? x.Farmer.ContactName : x.Email),
                Amount = null,
                CreatedAt = x.LastWarnedAt ?? x.CreatedAt
            })
            .ToListAsync();

        return newUsers
            .Concat(completedJobs)
            .Concat(resolvedDisputes)
                .Concat(walletTransactions)
            .Concat(lockedAccounts)
            .OrderByDescending(x => x.CreatedAt)
            .Take(activityLimit)
            .ToList();
    }
}
