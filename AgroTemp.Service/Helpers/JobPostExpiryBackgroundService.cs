using AgroTemp.Domain.Context;
using AgroTemp.Domain.Entities;
using AgroTemp.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AgroTemp.Service.Helpers
{
    public class JobPostExpiryBackgroundService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<JobPostExpiryBackgroundService> _logger;

        public JobPostExpiryBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<JobPostExpiryBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("JobPostExpiryBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CloseExpiredJobPostsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while closing expired job posts.");
                }

                await Task.Delay(Interval, stoppingToken);
            }

            _logger.LogInformation("JobPostExpiryBackgroundService stopped.");
        }

        private async Task CloseExpiredJobPostsAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<AgroTempDbContext>>();

            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);
            var currentTime = TimeOnly.FromDateTime(now);

            var expiredJobPosts = await unitOfWork.GetRepository<JobPost>()
                .GetListAsync(
                    predicate: jp =>
                        jp.StatusId == (int)JobPostStatus.Published &&
                        jp.EndDate.HasValue &&
                        (jp.EndDate.Value < today ||
                        jp.EndDate.Value == today && jp.EndTime <= currentTime),
                    include: null,
                    orderBy: null);

            if (expiredJobPosts == null || !expiredJobPosts.Any())
                return;

            _logger.LogInformation("Closing {Count} expired job post(s).", expiredJobPosts.Count);

            foreach (var jobPost in expiredJobPosts)
            {
                jobPost.StatusId = (int)JobPostStatus.Closed;
                unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
            }

            await unitOfWork.SaveChangesAsync();
        }
    }
}
