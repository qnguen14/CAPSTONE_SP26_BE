using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Domain.Metadata;
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
                        include: ja => ja
                            .Include(j => j.Worker)
                                .ThenInclude(w => w.User)
                            .Include(j => j.JobPost.Farmer)
                            .Include(j => j.JobPost.Farm),
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

        public async Task<List<JobApplicationDTO>> GetJobApplicationsByWorker()
        {
            try
            {
                var userId = GetCurrentUserId();

                var worker = await _unitOfWork.GetRepository<Worker>()
                    .FirstOrDefaultAsync(predicate: w => w.UserId == userId);

                if (worker == null)
                    throw new KeyNotFoundException("Worker profile not found for the current user.");

                var jobApplications = await _unitOfWork.GetRepository<JobApplication>()
                    .GetListAsync(
                        predicate: ja => ja.WorkerId == worker.Id,
                        include: ja => ja
                            .Include(j => j.Worker)
                                .ThenInclude(w => w.User)
                            .Include(j => j.JobPost.Farmer)
                            .Include(j => j.JobPost.Farm),
                        orderBy: ja => ja.OrderByDescending(x => x.AppliedAt));

                if (jobApplications == null || !jobApplications.Any())
                    return new List<JobApplicationDTO>();

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
                        include: ja => ja
                            .Include(j => j.Worker)
                                .ThenInclude(w => w.User)
                            .Include(j => j.JobPost.Farmer)
                            .Include(j => j.JobPost.Farm));

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

        public async Task<PaginatedResponse<JobApplicationDTO>> GetJobApplicationsByJobPostId(Guid jobPostId, Guid farmerProfileId, int? statusId, bool includeAll, int page, int limit)
        {
            try
            {
                page = page < 1 ? 1 : page;
                limit = limit <= 0 ? 10 : limit;
                var skip = (page - 1) * limit;

                var statusFilter = statusId ?? (int)ApplicationStatus.Pending;

                System.Linq.Expressions.Expression<Func<JobApplication, bool>> predicate = ja =>
                    ja.JobPostId == jobPostId &&
                    ja.JobPost.FarmerId == farmerProfileId &&
                    (includeAll || ja.StatusId == statusFilter);

                var total = await _unitOfWork.GetRepository<JobApplication>().CountAsync(predicate);

                var query = _unitOfWork.GetRepository<JobApplication>().CreateBaseQuery(
                    predicate: predicate,
                    orderBy: ja => ja.OrderBy(x => x.AppliedAt),
                    include: ja => ja
                        .Include(j => j.Worker)
                            .ThenInclude(w => w.User)
                        .Include(j => j.JobPost.Farmer)
                        .Include(j => j.JobPost.Farm),
                    asNoTracking: true);

                var jobApplications = await query.Skip(skip).Take(limit).ToListAsync();

                return new PaginatedResponse<JobApplicationDTO>
                {
                    Data = _mapper.JobApplicationsToJobApplicationDtos(jobApplications),
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        Limit = limit,
                        Total = total,
                        TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit)
                    }
                };
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
                    .FirstOrDefaultAsync(predicate: w => w.UserId == userId, include: w => w.Include(x => x.User));

                if (worker == null)
                {
                    throw new Exception("Worker profile not found for the current user.");
                }

                // Fetch job post early for capacity check and notification reuse
                var jobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == request.JobPostId,
                        include: q => q.Include(jp => jp.Farmer));

                if (jobPost == null)
                    throw new KeyNotFoundException("Job post not found.");

                if (jobPost.WorkersAccepted >= jobPost.WorkersNeeded)
                    throw new InvalidOperationException("This job has already reached its required worker capacity.");

                if (worker.User.WarningCount > 3)
                {
                    throw new UnauthorizedAccessException("Worker over warning dispute");
                }

                if (DateTime.Now <= worker.User.LastWarnedAt?.AddDays(worker.User.WarningCount * 3))
                {
                    throw new UnauthorizedAccessException($"Worker can't apply job post before {worker.User.LastWarnedAt?.AddDays(worker.User.WarningCount * 3)}");
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

                if (jobPost.Farmer != null)
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

                    if (existingJobApplication.JobPost.WorkersAccepted >= existingJobApplication.JobPost.WorkersNeeded)
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

        public async Task<JobApplicationDTO> CancelJobApplication(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var existingJobApplication = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(
                        predicate: ja => ja.Id == id,
                        include: ja => ja.Include(j => j.Worker).Include(j => j.JobPost).ThenInclude(jp => jp.Farmer));

                if (existingJobApplication == null)
                    throw new KeyNotFoundException("Job application not found.");

                if (existingJobApplication.Worker.UserId != currentUserId)
                    throw new UnauthorizedAccessException("You are only authorized to cancel your own applications.");

                if (existingJobApplication.StatusId == (int)ApplicationStatus.Cancelled ||
                    existingJobApplication.StatusId == (int)ApplicationStatus.Rejected)
                {
                    throw new InvalidOperationException("Cannot cancel an application that is already cancelled or rejected.");
                }

                if (existingJobApplication.StatusId == (int)ApplicationStatus.Accepted)
                {
                    existingJobApplication.JobPost.WorkersAccepted -= 1;

                    if (existingJobApplication.JobPost.StatusId == (int)JobPostStatus.Closed)
                    {
                        existingJobApplication.JobPost.StatusId = (int)JobPostStatus.Published;
                    }

                    if (existingJobApplication.JobPost.Farmer != null)
                    {
                        await _notificationService.CreateAsync(new CreateNotificationRequest
                        {
                            UserId = existingJobApplication.JobPost.Farmer.UserId,
                            Type = NotificationType.JobAcceptance,
                            Title = "Công nhân hủy nhận việc",
                            Message = $"Một công nhân đã hủy nhận việc cho \"{existingJobApplication.JobPost.Title}\". Vị trí công việc này đã được mở lại.",
                            RelatedEntityId = existingJobApplication.JobPostId
                        });
                    }
                }

                existingJobApplication.StatusId = (int)ApplicationStatus.Cancelled;

                _unitOfWork.GetRepository<JobApplication>().UpdateAsync(existingJobApplication);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.JobApplicationToJobApplicationDto(existingJobApplication);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobApplicationDTO> CancelJobApplicationForFarmer(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var existingJobApplication = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(
                        predicate: ja => ja.Id == id,
                        include: ja => ja.Include(j => j.Worker).Include(j => j.JobPost).ThenInclude(jp => jp.Farmer));

                if (existingJobApplication == null)
                    throw new KeyNotFoundException("Job application not found.");

                if (existingJobApplication.JobPost.Farmer.UserId != currentUserId)
                    throw new UnauthorizedAccessException("You are only authorized to cancel applications for your own job posts.");

                if (existingJobApplication.StatusId == (int)ApplicationStatus.Cancelled ||
                    existingJobApplication.StatusId == (int)ApplicationStatus.Rejected)
                {
                    throw new InvalidOperationException("Cannot cancel an application that is already cancelled or rejected.");
                }

                if (existingJobApplication.StatusId == (int)ApplicationStatus.Accepted)
                {
                    existingJobApplication.JobPost.WorkersAccepted -= 1;
                    if (existingJobApplication.JobPost.StatusId == (int)JobPostStatus.Closed)
                    {
                        existingJobApplication.JobPost.StatusId = (int)JobPostStatus.Published;
                    }

                    if (existingJobApplication.Worker != null)
                    {
                        await _notificationService.CreateAsync(new CreateNotificationRequest
                        {
                            UserId = existingJobApplication.Worker.UserId,
                            Type = NotificationType.JobAcceptance,
                            Title = "Đơn tuyển dụng bị HỦY",
                            Message = $"Đơn tuyển dụng của bạn cho \"{existingJobApplication.JobPost.Title}\" đã bị hủy bởi người nông dân. Vị trí công việc này đã được mở lại.",
                            RelatedEntityId = existingJobApplication.JobPostId
                        });
                    }
                }

                existingJobApplication.StatusId = (int)ApplicationStatus.Cancelled;

                _unitOfWork.GetRepository<JobApplication>().UpdateAsync(existingJobApplication);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.JobApplicationToJobApplicationDto(existingJobApplication);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResponse<JobApplicationDTO>> GetJobApplicationsByFarmer(int? statusId, bool includeAll, int page, int limit)
        {
            try
            {
                page = page < 1 ? 1 : page;
                limit = limit <= 0 ? 10 : limit;
                var skip = (page - 1) * limit;

                var statusFilter = statusId ?? (int)ApplicationStatus.Pending;

                var currentUserId = GetCurrentUserId();
                var farmerProfile = await _unitOfWork.GetRepository<Farmer>()
                    .FirstOrDefaultAsync(predicate: fp => fp.UserId == currentUserId);

                if (farmerProfile == null)
                {
                    throw new KeyNotFoundException("Farmer profile not found for the current user.");
                }

                System.Linq.Expressions.Expression<Func<JobApplication, bool>> predicate;
                if (includeAll)
                {
                    predicate = ja => ja.JobPost.FarmerId == farmerProfile.Id;
                }
                else
                {
                    predicate = ja => ja.JobPost.FarmerId == farmerProfile.Id && ja.StatusId == statusFilter;
                }

                var total = await _unitOfWork.GetRepository<JobApplication>().CountAsync(predicate);

                var query = _unitOfWork.GetRepository<JobApplication>().CreateBaseQuery(
                    predicate: predicate,
                    orderBy: ja => ja.OrderBy(x => x.AppliedAt),
                    include: ja => ja
                        .Include(j => j.Worker)
                            .ThenInclude(w => w.User)
                        .Include(j => j.JobPost.Farmer)
                        .Include(j => j.JobPost.Farm),
                    asNoTracking: true);

                var jobApplications = await query.Skip(skip).Take(limit).ToListAsync();

                return new PaginatedResponse<JobApplicationDTO>
                {
                    Data = _mapper.JobApplicationsToJobApplicationDtos(jobApplications),
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        Limit = limit,
                        Total = total,
                        TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit)
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
