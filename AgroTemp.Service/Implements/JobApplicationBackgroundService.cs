using AgroTemp.Domain.Context;
using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.Entities;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AgroTemp.Service.Implements
{
    public class JobApplicationBackgroundService : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<JobApplicationBackgroundService> _logger;

        public JobApplicationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<JobApplicationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("JobApplicationBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessUrgentJobApplicationsAsync(stoppingToken);
                    await Task.Delay(PollInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while auto-accepting urgent job applications.");
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                }
            }

            _logger.LogInformation("JobApplicationBackgroundService stopped.");
        }

        private async Task ProcessUrgentJobApplicationsAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<AgroTempDbContext>>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var urgentPosts = await unitOfWork.GetRepository<JobPost>()
                .GetListAsync(
                    predicate: jp =>
                        jp.IsUrgent &&
                        jp.StatusId == (int)JobPostStatus.Published &&
                        jp.WorkersAccepted < jp.WorkersNeeded,
                    include: q => q
                        .Include(jp => jp.Farmer)
                        .Include(jp => jp.JobPostDays)
                        .Include(jp => jp.JobApplications)
                            .ThenInclude(ja => ja.Worker),
                    orderBy: jp => jp.OrderBy(x => x.CreatedAt));

            if (urgentPosts == null || !urgentPosts.Any())
                return;

            var now = DateTime.UtcNow;
            var hasChanges = false;

            foreach (var post in urgentPosts)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var pendingApplications = post.JobApplications
                    .Where(ja => ja.StatusId == (int)ApplicationStatus.Pending)
                    .OrderBy(ja => ja.AppliedAt)
                    .ToList();

                if (!pendingApplications.Any())
                    continue;

                var remainingSlots = post.WorkersNeeded - post.WorkersAccepted;
                if (remainingSlots <= 0)
                    continue;

                if (post.JobTypeId == (int)JobType.Daily)
                {
                    foreach (var application in pendingApplications)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (post.StatusId == (int)JobPostStatus.Closed)
                            break;

                        var requestedDates = (application.WorkDates ?? new List<DateTime>())
                            .Select(d => DateOnly.FromDateTime(d))
                            .Distinct()
                            .ToList();

                        if (!requestedDates.Any())
                            continue;

                        var canAccept = true;
                        foreach (var date in requestedDates)
                        {
                            var dayEntry = post.JobPostDays.FirstOrDefault(d => d.WorkDate == date);
                            if (dayEntry == null || dayEntry.WorkersAccepted >= dayEntry.WorkersNeeded)
                            {
                                canAccept = false;
                                break;
                            }
                        }

                        if (!canAccept)
                            continue;

                        foreach (var date in requestedDates)
                        {
                            var dayEntry = post.JobPostDays.First(d => d.WorkDate == date);
                            dayEntry.WorkersAccepted += 1;
                        }

                        application.StatusId = (int)ApplicationStatus.Accepted;
                        application.RespondedAt = now;
                        application.ResponseMessage = "Đơn tuyển dụng của bạn đã được tự động chấp nhận do công việc này đang cần người gấp.";
                        unitOfWork.GetRepository<JobApplication>().UpdateAsync(application);
                        hasChanges = true;

                        if (application.Worker != null)
                        {
                            try
                            {
                                await notificationService.CreateAsync(new CreateNotificationRequest
                                {
                                    UserId = application.Worker.UserId,
                                    Type = NotificationType.JobAcceptance,
                                    Title = "Đơn tuyển dụng được CHẤP NHẬN",
                                    Message = $"Đơn tuyển dụng của bạn cho \"{post.Title}\" đã được tự động chấp nhận vì đây là công việc khẩn cấp.",
                                    RelatedEntityId = application.JobPostId
                                });
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to send auto-accept notification for application {ApplicationId}", application.Id);
                            }
                        }

                        post.WorkersAccepted = post.JobPostDays.Sum(d => d.WorkersAccepted);

                        var allDaysFull = post.JobPostDays.Any() && post.JobPostDays.All(d => d.WorkersAccepted >= d.WorkersNeeded);
                        if (allDaysFull)
                        {
                            post.StatusId = (int)JobPostStatus.Closed;
                        }
                    }

                    unitOfWork.GetRepository<JobPost>().UpdateAsync(post);
                    continue;
                }

                var toAccept = pendingApplications.Take(remainingSlots).ToList();

                foreach (var application in toAccept)
                {
                    application.StatusId = (int)ApplicationStatus.Accepted;
                    application.RespondedAt = now;
                    application.ResponseMessage = "Đơn tuyển dụng của bạn đã được tự động chấp nhận do công việc này đang cần người gấp.";
                    unitOfWork.GetRepository<JobApplication>().UpdateAsync(application);
                    hasChanges = true;

                    if (application.Worker != null)
                    {
                        try
                        {
                            await notificationService.CreateAsync(new CreateNotificationRequest
                            {
                                UserId = application.Worker.UserId,
                                Type = NotificationType.JobAcceptance,
                                Title = "Đơn tuyển dụng được CHẤP NHẬN",
                                Message = $"Đơn tuyển dụng của bạn cho \"{post.Title}\" đã được tự động chấp nhận vì đây là công việc khẩn cấp.",
                                RelatedEntityId = application.JobPostId
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send auto-accept notification for application {ApplicationId}", application.Id);
                        }
                    }
                }

                post.WorkersAccepted += toAccept.Count;
                if (post.WorkersAccepted >= post.WorkersNeeded)
                {
                    post.StatusId = (int)JobPostStatus.Closed;
                }

                unitOfWork.GetRepository<JobPost>().UpdateAsync(post);
            }

            if (hasChanges)
            {
                var saved = await unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Auto-accepted urgent applications. SaveChanges: {Count}", saved);
            }
        }
    }
}
