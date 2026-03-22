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
                        include: ja => ja.Include(j => j.Worker),
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
                        include: ja => ja.Include(j => j.Worker));

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
                jobApplication.StatusId = (int)ApplicationStatus.Pending;

                await _unitOfWork.GetRepository<JobApplication>().InsertAsync(jobApplication);
                await _unitOfWork.SaveChangesAsync();

                // Get the job post and farmer information
                var jobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == jobApplication.JobPostId,
                        include: q => q.Include(jp => jp.Farmer));

                if (jobPost != null && jobPost.Farmer != null)
                {
                    // Send notification to the farmer
                    var notificationRequest = new CreateNotificationRequest
                    {
                        UserId = jobPost.Farmer.UserId,
                        Type = NotificationType.JobAcceptance,
                        Title = "New Job Application",
                        Message = $"A worker has applied for your job post: {jobPost.Title}",
                        RelatedEntityId = jobApplication.Id
                    };

                    await _notificationService.CreateAsync(notificationRequest);
                }

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
                        include: ja => ja.Include(j => j.Worker).Include(j => j.JobPost));
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

                    _unitOfWork.GetRepository<JobPost>().UpdateAsync(existingJobApplication.JobPost);
                }

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
    }
}
