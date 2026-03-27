using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Service.Implements
{
    public class JobApplicationService : BaseService<JobApplication>, IJobApplicationService
    {
        private readonly INotificationService _notificationService;

        public JobApplicationService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper,
            INotificationService notificationService) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<List<JobApplicationDTO>> GetAllJobApplications()
        {
            try
            {
                var jobApplications = await _unitOfWork.GetRepository<JobApplication>()
                    .GetListAsync(
                        predicate: null,
                        include: ja => ja.Include(j => j.Worker).Include(j => j.JobPost.Farmer).Include(j => j.JobPost.Farm),
                        orderBy: ja => ja.OrderBy(x => x.AppliedAt));

                if (jobApplications == null || !jobApplications.Any())
                {
                    return null;
                }

                var result = _mapper.JobApplicationsToJobApplicationDtos(jobApplications);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobApplicationDTO> GetJobApplicationById(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var jobApplication = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(
                        predicate: ja => ja.Id == guid,
                        include: ja => ja.Include(j => j.Worker).Include(j => j.JobPost.Farmer).Include(j => j.JobPost.Farm));

                if (jobApplication == null)
                {
                    return null;
                }

                var result = _mapper.JobApplicationToJobApplicationDto(jobApplication);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobApplicationDTO>> GetJobApplicationsByJobPostId(Guid jobPostId, Guid farmerProfileId, int? statusId, bool includeAll)
        {
            try
            {
                var statusFilter = statusId ?? (int)ApplicationStatus.Pending;

                var jobApplications = await _unitOfWork.GetRepository<JobApplication>()
                    .GetListAsync(predicate: ja =>
                                    ja.JobPostId == jobPostId &&
                                    ja.JobPost.FarmerId == farmerProfileId &&
                                    (includeAll || ja.StatusId == statusFilter),
                                include: ja => ja.Include(j => j.Worker)
                                                .Include(j => j.JobPost.Farmer)
                                                .Include(j => j.JobPost.Farm),
                                orderBy: ja => ja.OrderBy(x => x.AppliedAt));

                if (jobApplications == null || !jobApplications.Any())
                {
                    return new List<JobApplicationDTO>();
                }

                return _mapper.JobApplicationsToJobApplicationDtos(jobApplications);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobApplicationDTO> CreateJobApplication(CreateJobApplicationRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var worker = await _unitOfWork.GetRepository<Worker>()
                    .FirstOrDefaultAsync(predicate:w => w.UserId == userId);

                if (worker == null)
                {
                    throw new Exception("Worker profile not found for the current user.");
                }

                var jobApplication = _mapper.CreateJobApplicationRequestToJobApplication(request);

                // Set defaults
                jobApplication.WorkerId = worker.Id;
                jobApplication.Id = Guid.NewGuid();
                jobApplication.AppliedAt = DateTime.UtcNow;
                jobApplication.RespondedAt = null;
                jobApplication.ResponseMessage = null;
                jobApplication.StatusId = (int)ApplicationStatus.Pending;

                await _unitOfWork.GetRepository<JobApplication>().InsertAsync(jobApplication);

                // Get the job post and farmer information for the notification
                var jobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == jobApplication.JobPostId,
                        include: q => q.Include(jp => jp.Farmer));

                if (jobPost != null && jobPost.Farmer != null)
                {
                    var notificationRequest = new CreateNotificationRequest
                    {
                        UserId = jobPost.Farmer.UserId,
                        Type = NotificationType.JobAcceptance,
                        Title = "Đơn tuyển dụng mới",
                        Message = $"Một công nhân đã nộp đơn tuyển dụng cho bài đăng công việc: {jobPost.Title}",
                        RelatedEntityId = jobApplication.Id
                    };

                    await _notificationService.CreateAsync(notificationRequest);
                }

                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.JobApplicationToJobApplicationDto(jobApplication);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobApplicationDTO> UpdateJobApplication(Guid id, UpdateJobApplicationRequest request)
        {
            try
            {
                var existingJobApplication = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(
                        predicate: ja => ja.Id == id);

                if (existingJobApplication == null)
                {
                    return null;
                }

                _mapper.UpdateJobApplicationRequestToJobApplication(request, existingJobApplication);
                _unitOfWork.GetRepository<JobApplication>().UpdateAsync(existingJobApplication);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.JobApplicationToJobApplicationDto(existingJobApplication);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteJobApplication(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var existingJobApplication = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(
                        predicate: ja => ja.Id == guid,
                        include: ja => ja.Include(j => j.Worker));

                if (existingJobApplication == null)
                {
                    return false;
                }

                _unitOfWork.GetRepository<JobApplication>().DeleteAsync(existingJobApplication);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobApplicationDTO> RespondJobApplication(string id, RespondJobApplicationRequest request)
        {
            try
            {
                var guid = Guid.Parse(id);
                var existingJobApplication = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(
                        predicate: ja => ja.Id == guid,
                        include: ja => ja.Include(j => j.Worker).Include(j => j.JobPost).ThenInclude(jp => jp.Farmer));
                if (existingJobApplication == null)
                {
                    return null;
                }
                
                existingJobApplication.StatusId = request.StatusId;
                existingJobApplication.RespondedAt = request.RespondedAt;
                existingJobApplication.ResponseMessage = request.ResponseMessage;

                if (request.StatusId == (int)ApplicationStatus.Accepted)
                {
                    existingJobApplication.JobPost.WorkersAccepted += 1;

                    if (existingJobApplication.JobPost.WorkersAccepted == existingJobApplication.JobPost.WorkersNeeded)
                    {
                        existingJobApplication.JobPost.StatusId = (int)JobPostStatus.Closed;
                    }
                }

                _unitOfWork.GetRepository<JobApplication>().UpdateAsync(existingJobApplication);

                if (existingJobApplication.Worker != null)
                {
                    var statusMessage = request.StatusId == (int)ApplicationStatus.Accepted 
                        ? "chấp nhận" 
                        : "từ chối";

                    var notificationRequest = new CreateNotificationRequest
                    {
                        UserId = existingJobApplication.Worker.UserId,
                        Type = NotificationType.JobAcceptance,
                        Title = $"Đơn tuyển dụng {statusMessage.ToUpper()}",
                        Message = $"Đơn tuyển dụng của bạn cho \"{existingJobApplication.JobPost.Title}\" đã được {statusMessage}.",
                        RelatedEntityId = existingJobApplication.JobPostId
                    };

                    await _notificationService.CreateAsync(notificationRequest);
                }

                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.JobApplicationToJobApplicationDto(existingJobApplication);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobApplicationDTO>> AutoAcceptUrgentJobApplicationsAsync(List<Guid> jobApplicationIds)
        {
            try
            {
                if (jobApplicationIds == null || !jobApplicationIds.Any())
                    throw new ArgumentException("No job application IDs provided.");

                var currentUserId = GetCurrentUserId();

                var applications = await _unitOfWork.GetRepository<JobApplication>()
                    .GetListAsync(
                        predicate: ja => jobApplicationIds.Contains(ja.Id),
                        include: q => q
                            .Include(ja => ja.Worker)
                            .Include(ja => ja.JobPost)
                            .ThenInclude(jp => jp.Farmer));

                if (applications == null || !applications.Any())
                    throw new KeyNotFoundException("No job applications found for the provided IDs.");

                var jobPost = applications.First().JobPost;

                if (jobPost == null)
                    throw new KeyNotFoundException("Job post not found.");

                if (jobPost.Farmer == null || jobPost.Farmer.UserId != currentUserId)
                    throw new UnauthorizedAccessException("Only the farmer who owns this job post can auto-accept applications.");

                if (!jobPost.IsUrgent)
                    throw new InvalidOperationException("Auto-accept is only available for urgent job posts.");

                var remainingSlots = jobPost.WorkersNeeded - jobPost.WorkersAccepted;
                if (remainingSlots <= 0)
                    throw new InvalidOperationException("This job post has already reached its required number of workers.");

                var pendingApplications = applications
                    .Where(ja => ja.StatusId == (int)ApplicationStatus.Pending)
                    .OrderBy(ja => ja.AppliedAt)
                    .Take(remainingSlots)
                    .ToList();

                if (!pendingApplications.Any())
                    throw new InvalidOperationException("None of the provided applications are pending.");

                foreach (var application in pendingApplications)
                {
                    application.StatusId = (int)ApplicationStatus.Accepted;
                    application.RespondedAt = DateTime.UtcNow;
                    application.ResponseMessage = "Đơn tuyển dụng của bạn đã được tự động chấp nhận do công việc này đang cần người gấp.";

                    _unitOfWork.GetRepository<JobApplication>().UpdateAsync(application);

                    if (application.Worker != null)
                    {
                        await _notificationService.CreateAsync(new CreateNotificationRequest
                        {
                            UserId = application.Worker.UserId,
                            Type = NotificationType.JobAcceptance,
                            Title = "Đơn tuyển dụng được CHẤP NHẬN",
                            Message = $"Đơn tuyển dụng của bạn cho \"{jobPost.Title}\" đã được tự động chấp nhận vì đây là công việc khẩn cấp.",
                            RelatedEntityId = application.JobPostId
                        });
                    }
                }

                jobPost.WorkersAccepted += pendingApplications.Count;

                if (jobPost.WorkersAccepted >= jobPost.WorkersNeeded)
                    jobPost.StatusId = (int)JobPostStatus.Closed;

                _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.JobApplicationsToJobApplicationDtos(pendingApplications);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
