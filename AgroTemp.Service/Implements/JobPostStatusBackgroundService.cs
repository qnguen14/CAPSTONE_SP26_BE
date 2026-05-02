using AgroTemp.Domain.Context;
using AgroTemp.Domain.Entities;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AgroTemp.Service.Implements
{
    public class JobPostStatusBackgroundService : BackgroundService
    {
        private static readonly TimeSpan MaxSleepDuration = TimeSpan.FromSeconds(5);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<JobPostStatusBackgroundService> _logger;

        public JobPostStatusBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<JobPostStatusBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("JobPostStatusBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var delay = await ProcessAndGetNextDelayAsync();
                    var sleepDuration = delay <= TimeSpan.Zero
                        ? TimeSpan.Zero
                        : (delay < MaxSleepDuration ? delay : MaxSleepDuration);

                    _logger.LogInformation(
                        "Next status check in {Seconds:F0}s (at {WakeAt:HH:mm:ss} UTC).",
                        sleepDuration.TotalSeconds,
                        DateTime.UtcNow.Add(sleepDuration));

                    await Task.Delay(sleepDuration, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing job post statuses.");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }

            _logger.LogInformation("JobPostStatusBackgroundService stopped.");
        }

        private async Task<TimeSpan> ProcessAndGetNextDelayAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<AgroTempDbContext>>();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            var now = DateTime.UtcNow;

            var nextExpiry = await CloseExpiredJobPostsAsync(unitOfWork, walletService, now);
            var nextUnfilledCancel = await CancelUnfilledJobPostsAsync(unitOfWork, walletService, now);
            var nextStartTime = await StartInProgressJobPostsAsync(unitOfWork, now);

            var nextEvent = Min(Min(nextExpiry, nextUnfilledCancel), nextStartTime);

            if (nextEvent.HasValue)
            {
                var delay = nextEvent.Value - now;
                return delay;
            }

            return MaxSleepDuration;
        }

        private async Task<DateTime?> CloseExpiredJobPostsAsync(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IWalletService walletService,
            DateTime now)
        {
            var publishedPosts = await unitOfWork.GetRepository<JobPost>()
                .GetListAsync(
                    predicate: jp =>
                        jp.StatusId == (int)JobPostStatus.Published &&
                        jp.EndDate.HasValue,
                    include: jp => jp.Include(p => p.Farmer),
                    orderBy: null);

            if (publishedPosts == null || !publishedPosts.Any())
                return null;

            var expiredPosts = new List<JobPost>();
            DateTime? nextExpiry = null;

            foreach (var post in publishedPosts)
            {
                var expiresAt = post.EndDate!.Value.ToDateTime(post.EndTime);

                if (expiresAt <= now)
                    expiredPosts.Add(post);
                else if (nextExpiry == null || expiresAt < nextExpiry.Value)
                    nextExpiry = expiresAt;
            }

            if (expiredPosts.Any())
            {
                _logger.LogInformation("Cancelling {Count} expired job post(s).", expiredPosts.Count);

                foreach (var post in expiredPosts)
                {
                    post.StatusId = (int)JobPostStatus.Cancelled;
                    unitOfWork.GetRepository<JobPost>().UpdateAsync(post);

                    if (post.Farmer != null)
                    {
                        var lockedAmount = post.JobTypeId == (int)JobType.PerJob
                            ? post.WageAmount
                            : post.WageAmount * post.WorkersNeeded;

                        if (lockedAmount > 0)
                        {
                            try
                            {
                                await walletService.RefundLockedAmountForJobPostAsync(
                                    post.Farmer.UserId,
                                    post.Id,
                                    lockedAmount);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to refund locked amount for JobPost {JobPostId} (Farmer: {FarmerId}). Continuing.", post.Id, post.Farmer.UserId);
                            }
                        }
                    }
                }

                var saved = await unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Saved changes after cancelling posts: {SavedCount}.", saved);
            }

            return nextExpiry;
        }

        private async Task<DateTime?> CancelUnfilledJobPostsAsync(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IWalletService walletService,
            DateTime now)
        {
            var publishedWithStart = await unitOfWork.GetRepository<JobPost>()
                .GetListAsync(
                    predicate: jp =>
                        jp.StatusId == (int)JobPostStatus.Published &&
                        jp.StartDate.HasValue,
                    include: jp => jp.Include(p => p.Farmer),
                    orderBy: null);

            if (publishedWithStart == null || !publishedWithStart.Any())
                return null;

            var toCancel = new List<JobPost>();
            DateTime? nextStart = null;

            foreach (var post in publishedWithStart)
            {
                var startsAt = post.StartDate!.Value.ToDateTime(post.StartTime);

                if (startsAt > now)
                {
                    if (nextStart == null || startsAt < nextStart.Value)
                        nextStart = startsAt;
                    continue;
                }

                if (post.WorkersAccepted >= post.WorkersNeeded)
                    continue; // not unfilled

                toCancel.Add(post);
            }

            if (toCancel.Any())
            {
                _logger.LogInformation("Cancelling {Count} unfilled job post(s) that missed start time.", toCancel.Count);

                foreach (var post in toCancel)
                {
                    post.StatusId = (int)JobPostStatus.Cancelled;
                    unitOfWork.GetRepository<JobPost>().UpdateAsync(post);

                    if (post.Farmer != null)
                    {
                        var lockedAmount = post.JobTypeId == (int)JobType.PerJob
                            ? post.WageAmount
                            : post.WageAmount * post.WorkersNeeded;

                        if (lockedAmount > 0)
                        {
                            try
                            {
                                await walletService.RefundLockedAmountForJobPostAsync(
                                    post.Farmer.UserId,
                                    post.Id,
                                    lockedAmount);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to refund locked amount for unfilled JobPost {JobPostId} (Farmer: {FarmerId}). Continuing.", post.Id, post.Farmer.UserId);
                            }
                        }
                    }
                }

                var saved = await unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Saved changes after cancelling unfilled posts: {SavedCount}.", saved);
            }

            return nextStart;
        }

        private async Task<DateTime?> StartInProgressJobPostsAsync(
            IUnitOfWork<AgroTempDbContext> unitOfWork, DateTime now)
        {
            var eligiblePosts = await unitOfWork.GetRepository<JobPost>()
                .GetListAsync(
                    predicate: jp =>
                        (jp.StatusId == (int)JobPostStatus.Published ||
                         jp.StatusId == (int)JobPostStatus.Closed) &&
                        jp.StartDate.HasValue &&
                        jp.WorkersAccepted >= jp.WorkersNeeded,
                    include: jp => jp
                        .Include(p => p.JobApplications)
                        .Include(p => p.JobPostDays),
                    orderBy: null);

            if (eligiblePosts == null || !eligiblePosts.Any())
                return null;

            var readyPosts = new List<JobPost>();
            DateTime? nextStart = null;

            foreach (var post in eligiblePosts)
            {
                var startsAt = post.StartDate!.Value.ToDateTime(post.StartTime);

                if (startsAt > now)
                {
                    if (nextStart == null || startsAt < nextStart.Value)
                        nextStart = startsAt;
                    continue;
                }

                if (post.JobPostDays != null && post.JobPostDays.Count > 0)
                {
                    var coveredDates = post.JobApplications
                        .Where(ja => ja.StatusId == (int)ApplicationStatus.Accepted && ja.WorkDates != null)
                        .SelectMany(ja => ja.WorkDates!.Select(d => DateOnly.FromDateTime(d)))
                        .ToHashSet();

                    if (!post.JobPostDays.All(day => coveredDates.Contains(day.WorkDate)))
                        continue;
                }

                readyPosts.Add(post);
            }

            if (readyPosts.Any())
            {
                _logger.LogInformation(
                    "Transitioning {Count} job post(s) to InProgress.", readyPosts.Count);

                foreach (var post in readyPosts)
                {
                    post.StatusId = (int)JobPostStatus.InProgress;
                    unitOfWork.GetRepository<JobPost>().UpdateAsync(post);
                }

                await unitOfWork.SaveChangesAsync();
            }

            return nextStart;
        }

        private static DateTime? Min(DateTime? a, DateTime? b)
        {
            if (a == null) return b;
            if (b == null) return a;
            return a.Value < b.Value ? a : b;
        }
    }
}
