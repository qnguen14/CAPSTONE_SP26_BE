using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Domain.Metadata;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Helpers;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Service.Implements
{
    public class JobPostService : BaseService<JobPost>, IJobPostService
    {
        private readonly IMapperlyMapper _mapper;
        private readonly IWalletService _walletService;
        private readonly INotificationService _notificationService;

        public JobPostService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper,
            IWalletService walletService,
            INotificationService notificationService) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _walletService = walletService;
            _notificationService = notificationService;
        }

        public async Task<List<JobPostDTO>> GetAllJobPosts()
        {
            try
            {
                var jobPosts = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: null,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobApplications)
                                .ThenInclude(ja => ja.Worker)
                                    .ThenInclude(w => w.User)
                            .Include(jp => jp.JobPostDays),
                        orderBy: jp => jp.OrderBy(x => x.Title));
                if (jobPosts == null || !jobPosts.Any())
                {
                    return null;
                }
                var result = _mapper.JobPostsToJobPostDtos(jobPosts);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobPostDTO> GetJobPostById(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var jobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == guid,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobSkillRequirements)
                                .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobApplications)
                                .ThenInclude(ja => ja.Worker)
                                    .ThenInclude(w => w.User)
                            .Include(jp => jp.JobPostDays));
                if (jobPost == null)
                {
                    return null;
                }
                var result = _mapper.JobPostToJobPostDto(jobPost);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobPostDTO>> GetJobPostsByFarmerId()
        {
            try
            {
                var userId = GetCurrentUserId();
                var farmer = await _unitOfWork.GetRepository<Farmer>()
                    .FirstOrDefaultAsync(predicate: f => f.UserId == userId);
                if (farmer == null)
                {
                    throw new UnauthorizedAccessException("User is not authorized to view these job posts.");
                }

                var jobPosts = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.FarmerId == farmer.Id,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobApplications)
                                .ThenInclude(ja => ja.Worker)
                                    .ThenInclude(w => w.User)
                            .Include(jp => jp.JobPostDays),
                        orderBy: jp => jp.OrderByDescending(x => x.CreatedAt));

                if (jobPosts == null || !jobPosts.Any())
                {
                    return new List<JobPostDTO>();
                }

                var result = _mapper.JobPostsToJobPostDtos(jobPosts);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobPostDTO>> GetFarmerJobHistory()
        {
            try
            {
                var userId = GetCurrentUserId();

                var farmer = await _unitOfWork.GetRepository<Farmer>()
                    .FirstOrDefaultAsync(predicate: f => f.UserId == userId);

                if (farmer == null)
                {
                    throw new UnauthorizedAccessException("User is not authorized to view job history.");
                }

                var jobPosts = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.FarmerId == farmer.Id &&
                                         (jp.StatusId == (int)JobPostStatus.Completed || jp.StatusId == (int)JobPostStatus.Cancelled),
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobApplications)
                                .ThenInclude(ja => ja.Worker)
                                    .ThenInclude(w => w.User)
                                    .Include(jp => jp.JobPostDays),
                        orderBy: jp => jp.OrderByDescending(x => x.CreatedAt));

                if (jobPosts == null || !jobPosts.Any())
                {
                    return new List<JobPostDTO>();
                }

                var result = _mapper.JobPostsToJobPostDtos(jobPosts);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobPostDTO>> GetJobPostsByStatus(JobPostStatus status)
        {
            try
            {
                var userId = GetCurrentUserId();
                var farmer = await _unitOfWork.GetRepository<Farmer>()
                    .FirstOrDefaultAsync(predicate: f => f.UserId == userId);

                if (farmer == null)
                {
                    throw new UnauthorizedAccessException("User is not authorized to view these job posts.");
                }

                var jobPosts = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.FarmerId == farmer.Id && jp.StatusId == (int)status,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobApplications)
                                .ThenInclude(ja => ja.Worker)
                                    .ThenInclude(w => w.User)
                                    .Include(jp => jp.JobPostDays),
                        orderBy: jp => jp.OrderByDescending(x => x.CreatedAt));

                if (jobPosts == null || !jobPosts.Any())
                {
                    return new List<JobPostDTO>();
                }

                var result = _mapper.JobPostsToJobPostDtos(jobPosts);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobPostDTO> CreateJobPost(CreateJobPostRequest request)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var farmer = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId, include: f => f.Include(x => x.User));

            if (farmer == null)
            {
                throw new UnauthorizedAccessException("Only farmers can create job posts.");
            }

            if (farmer.User.WarningCount > 3)
            {
                throw new UnauthorizedAccessException("Farmer over warning dispute");
            }

            if (DateTime.Now <= farmer.User.LastWarnedAt?.AddDays(farmer.User.WarningCount * 3))
            {
                throw new UnauthorizedAccessException($"Farmer can't create job post before {farmer.User.LastWarnedAt?.AddDays(farmer.User.WarningCount * 3)}");
            }

            var requestedSkillIds = request.SkillIds?
                .Distinct()
                .ToList() ?? new List<Guid>();

            var skills = requestedSkillIds.Any()
                ? await _unitOfWork.GetRepository<Skill>()
                    .GetListAsync(predicate: s => requestedSkillIds.Contains(s.Id))
                : new List<Skill>();

            if (skills.Count != requestedSkillIds.Count)
            {
                var foundSkillIds = skills.Select(s => s.Id).ToHashSet();
                var invalidSkillIds = requestedSkillIds.Where(id => !foundSkillIds.Contains(id));
                throw new ArgumentException($"Invalid skill ID(s): {string.Join(", ", invalidSkillIds)}");
            }

            if (request.StartDate.HasValue && request.StartDate.Value < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ArgumentException("Job start date cannot be in the past.");
            }

            if (request.WorkersNeeded <= 0)
            {
                throw new ArgumentException("WorkersNeeded must be greater than 0.");
            }

            var dayRequests = request.JobPostDays ?? new List<JobPostDayRequest>();
            if (request.JobTypeId == JobType.Daily)
            {
                if (!dayRequests.Any())
                {
                    throw new ArgumentException("JobPostDays are required for daily jobs.");
                }

                if (dayRequests.Any(d => d.WorkersNeeded <= 0))
                {
                    throw new ArgumentException("Each job post day must have WorkersNeeded greater than 0.");
                }

                var distinctDates = dayRequests.Select(d => d.WorkDate).Distinct().ToList();
                if (distinctDates.Count != dayRequests.Count)
                {
                    throw new ArgumentException("Duplicate work dates are not allowed.");
                }

                if (distinctDates.Any(d => d < DateOnly.FromDateTime(DateTime.UtcNow)))
                {
                    throw new ArgumentException("Job post days cannot be in the past.");
                }
            }

            var jobPost = _mapper.CreateJobPostRequestToJobPost(request);
            if (jobPost.Id == Guid.Empty)
            {
                jobPost.Id = Guid.NewGuid();
            }

            if (request.JobTypeId == JobType.Daily)
            {
                jobPost.JobPostDays = dayRequests
                    .Select(d => new JobPostDay
                    {
                        Id = Guid.NewGuid(),
                        JobPostId = jobPost.Id,
                        WorkDate = d.WorkDate,
                        WorkersNeeded = d.WorkersNeeded,
                        WorkersAccepted = 0
                    })
                    .ToList();
            }

            var workdays = ResolveBillableDays(request.StartDate, request.EndDate, dayRequests.Select(d => d.WorkDate));

            if (request.JobTypeId == JobType.PerJob)
            {
                jobPost.WorkersNeeded = request.WorkersNeeded;
            }
            else if (request.JobTypeId == JobType.Daily)
            {
                var totalWorkerDays = dayRequests.Sum(d => d.WorkersNeeded);
                jobPost.WorkersNeeded = totalWorkerDays;
            }

            jobPost.FarmerId = farmer.Id;
            jobPost.StatusId = request.StatusId;
            jobPost.CreatedAt = DateTime.UtcNow;
            jobPost.UpdatedAt = DateTime.UtcNow;
            jobPost.PublishedAt = request.PublishedAt;

            var totalWorkerDaysForLock = request.JobTypeId == JobType.Daily
                ? dayRequests.Sum(d => d.WorkersNeeded)
                : request.WorkersNeeded;
            var lockAmount = request.JobTypeId == JobType.PerJob
                ? request.WageAmount
                : request.WageAmount * totalWorkerDaysForLock;
            try
            {
                await _walletService.LockAmountForJobPostAsync(farmer.UserId, jobPost.Id, lockAmount);
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("Insufficient wallet balance to create job post. Please top up your wallet.", ex);
            }


            if (skills.Any())
            {
                var jobSkillRequirements = skills.Select(skill => new JobSkillRequirement
                {
                    Id = Guid.NewGuid(),
                    JobPostId = jobPost.Id,
                    SkillId = skill.Id,
                    RequiredLevelId = (int)ProficiencyLevel.Beginner,
                    IsMandatory = true
                }).ToList();

                await _unitOfWork.GetRepository<JobSkillRequirement>().InsertRangeAsync(jobSkillRequirements);
            }

            await _unitOfWork.GetRepository<JobPost>().InsertAsync(jobPost);
            await _unitOfWork.SaveChangesAsync();


            var createdJobPost = await _unitOfWork.GetRepository<JobPost>()
                .FirstOrDefaultAsync(
                    predicate: jp => jp.Id == jobPost.Id,
                    include: q => q
                        .Include(jp => jp.JobPostDays)
                        .Include(jp => jp.JobSkillRequirements)
                        .ThenInclude(jsr => jsr.Skill));

            var result = _mapper.JobPostToJobPostDto(createdJobPost ?? jobPost);

            return result;
        }

        public async Task<JobPostDTO> UpdateJobPost(Guid id, UpdateJobPostRequest request)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                if (!Enum.IsDefined(typeof(JobType), request.JobTypeId))
                {
                    throw new ArgumentException("Invalid job type.");
                }

                if (request.WorkersNeeded <= 0)
                {
                    throw new ArgumentException("WorkersNeeded must be greater than 0.");
                }

                if (request.JobCategoryId == Guid.Empty)
                {
                    throw new ArgumentException("JobCategoryId is required.");
                }

                if (request.FarmId == Guid.Empty)
                {
                    throw new ArgumentException("FarmId is required.");
                }

                var existingJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == id,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobPostDays)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill));

                if (existingJobPost == null)
                {
                    return null;
                }

                var originalWageAmount = existingJobPost.WageAmount;
                var originalWorkersNeeded = existingJobPost.WorkersNeeded;
                var originalJobTypeId = existingJobPost.JobTypeId;

                var existingCategory = await _unitOfWork.GetRepository<JobCategory>()
                    .FirstOrDefaultAsync(predicate: jc => jc.Id == request.JobCategoryId);

                if (existingCategory == null)
                {
                    throw new ArgumentException($"Invalid job category ID: {request.JobCategoryId}");
                }

                var existingFarm = await _unitOfWork.GetRepository<Farm>()
                    .FirstOrDefaultAsync(predicate: f => f.Id == request.FarmId);

                if (existingFarm == null)
                {
                    throw new ArgumentException($"Invalid farm ID: {request.FarmId}");
                }

                if (request.SkillIds != null)
                {
                    var requestedSkillIds = request.SkillIds.Distinct().ToList();
                    var existingSkillIds = existingJobPost.JobSkillRequirements.Select(jsr => jsr.SkillId).ToList();

                    var skillsToAdd = requestedSkillIds.Except(existingSkillIds).ToList();
                    var skillsToRemove = existingSkillIds.Except(requestedSkillIds).ToList();

                    if (skillsToAdd.Any())
                    {
                        var validSkills = await _unitOfWork.GetRepository<Skill>()
                            .GetListAsync(predicate: s => skillsToAdd.Contains(s.Id));

                        if (validSkills.Count != skillsToAdd.Count)
                        {
                            var foundSkillIds = validSkills.Select(s => s.Id).ToHashSet();
                            var invalidSkillIds = skillsToAdd.Where(sid => !foundSkillIds.Contains(sid));
                            throw new ArgumentException($"Invalid skill ID(s): {string.Join(", ", invalidSkillIds)}");
                        }

                        var newJobSkillRequirements = skillsToAdd.Select(skillId => new JobSkillRequirement
                        {
                            Id = Guid.NewGuid(),
                            JobPostId = existingJobPost.Id,
                            SkillId = skillId,
                            RequiredLevelId = (int)ProficiencyLevel.Beginner,
                            IsMandatory = true
                        }).ToList();

                        await _unitOfWork.GetRepository<JobSkillRequirement>().InsertRangeAsync(newJobSkillRequirements);
                    }

                    if (skillsToRemove.Any())
                    {
                        var requirementsToRemove = existingJobPost.JobSkillRequirements
                            .Where(jsr => skillsToRemove.Contains(jsr.SkillId))
                            .ToList();
                        _unitOfWork.GetRepository<JobSkillRequirement>().DeleteRangeAsync(requirementsToRemove);
                    }
                }

                var originalJobPostDays = existingJobPost.JobPostDays.ToList();
                var currentCreatedAt = existingJobPost.CreatedAt;
                var currentPublishedAt = existingJobPost.PublishedAt;
                _mapper.UpdateJobPostRequestToJobPost(request, existingJobPost);

                var dayRequests = request.JobPostDays ?? new List<JobPostDayRequest>();
                if (request.JobTypeId == (int)JobType.Daily)
                {
                    if (!dayRequests.Any())
                    {
                        throw new ArgumentException("JobPostDays are required for daily jobs.");
                    }

                    if (dayRequests.Any(d => d.WorkersNeeded <= 0))
                    {
                        throw new ArgumentException("Each job post day must have WorkersNeeded greater than 0.");
                    }

                    var distinctDates = dayRequests.Select(d => d.WorkDate).Distinct().ToList();
                    if (distinctDates.Count != dayRequests.Count)
                    {
                        throw new ArgumentException("Duplicate work dates are not allowed.");
                    }

                    var existingDayMap = existingJobPost.JobPostDays
                        .ToDictionary(d => d.WorkDate, d => d);

                    existingJobPost.JobPostDays = dayRequests
                        .Select(d =>
                        {
                            existingDayMap.TryGetValue(d.WorkDate, out var existingDay);
                            var accepted = existingDay?.WorkersAccepted ?? 0;
                            return new JobPostDay
                            {
                                Id = existingDay?.Id ?? Guid.NewGuid(),
                                JobPostId = existingJobPost.Id,
                                WorkDate = d.WorkDate,
                                WorkersNeeded = d.WorkersNeeded,
                                WorkersAccepted = Math.Min(accepted, d.WorkersNeeded)
                            };
                        })
                        .ToList();
                }
                else
                {
                    existingJobPost.JobPostDays = new List<JobPostDay>();
                }

                existingJobPost.WorkersNeeded = request.JobTypeId == (int)JobType.Daily
                    ? dayRequests.Sum(d => d.WorkersNeeded)
                    : request.WorkersNeeded;

                if (request.JobTypeId == (int)JobType.Daily)
                {
                    existingJobPost.WorkersAccepted = existingJobPost.JobPostDays.Sum(d => d.WorkersAccepted);
                }

                var oldTotalWorkerDays = originalJobTypeId == (int)JobType.Daily
                    ? originalJobPostDays.Sum(d => d.WorkersNeeded)
                    : originalWorkersNeeded;

                var newTotalWorkerDays = request.JobTypeId == (int)JobType.Daily
                    ? dayRequests.Sum(d => d.WorkersNeeded)
                    : existingJobPost.WorkersNeeded;

                var oldLockAmount = originalJobTypeId == (int)JobType.PerJob
                    ? originalWageAmount
                    : originalWageAmount * oldTotalWorkerDays;

                var newLockAmount = request.JobTypeId == (int)JobType.PerJob
                    ? request.WageAmount
                    : request.WageAmount * newTotalWorkerDays;

                var lockDelta = newLockAmount - oldLockAmount;

                if (lockDelta > 0)
                {
                    try
                    {
                        await _walletService.LockAmountForJobPostAsync(existingJobPost.Farmer.UserId, existingJobPost.Id, lockDelta);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new Exception("Insufficient wallet balance to update job post. Please top up your wallet.", ex);
                    }
                }
                else if (lockDelta < 0)
                {
                    await _walletService.RefundLockedAmountForJobPostAsync(existingJobPost.Farmer.UserId, existingJobPost.Id, Math.Abs(lockDelta));
                }

                // Keep system-managed timestamps stable on update.
                existingJobPost.CreatedAt = currentCreatedAt;
                existingJobPost.PublishedAt = currentPublishedAt;
                existingJobPost.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<JobPost>().UpdateAsync(existingJobPost);
                await _unitOfWork.SaveChangesAsync();

                var updatedJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == id,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobPostDays)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill));

                var result = _mapper.JobPostToJobPostDto(updatedJobPost ?? existingJobPost);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteJobPost(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var existingJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == guid);
                if (existingJobPost == null)
                {
                    return false;
                }
                _unitOfWork.GetRepository<JobPost>().DeleteAsync(existingJobPost);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobPostDTO> CancelJobPost(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var existingJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == id,
                        include: q => q.Include(jp => jp.Farmer));

                if (existingJobPost == null)
                    throw new KeyNotFoundException("Job post not found.");

                if (existingJobPost.Farmer.UserId != currentUserId)
                    throw new UnauthorizedAccessException("You are only authorized to cancel your own job posts.");

                if (existingJobPost.StatusId == (int)JobPostStatus.Cancelled || existingJobPost.StatusId == (int)JobPostStatus.Completed
                        || existingJobPost.StatusId == (int)JobPostStatus.InProgress || existingJobPost.StatusId == (int)JobPostStatus.Closed)
                    throw new InvalidOperationException("Job post cannot be cancelled in its current status.");

                if (existingJobPost.StartDate.HasValue && existingJobPost.StartDate.Value < DateOnly.FromDateTime(DateTime.UtcNow))
                    throw new InvalidOperationException("Cannot cancel a job post that has already started.");

                existingJobPost.StatusId = (int)JobPostStatus.Cancelled;
                _unitOfWork.GetRepository<JobPost>().UpdateAsync(existingJobPost);

                // Refund the locked amount back to the farmer's wallet
                var lockedAmount = existingJobPost.JobTypeId == (int)JobType.PerJob
                    ? existingJobPost.WageAmount
                    : existingJobPost.WageAmount * existingJobPost.WorkersNeeded;

                if (lockedAmount > 0)
                {
                    await _walletService.RefundLockedAmountForJobPostAsync(
                        existingJobPost.Farmer.UserId,
                        existingJobPost.Id,
                        lockedAmount);
                }

                var applicants = await _unitOfWork.GetRepository<JobApplication>()
                .GetListAsync(
                    predicate: ja => ja.JobPostId == id &&
                                     ja.StatusId != (int)ApplicationStatus.Cancelled &&
                                     ja.StatusId != (int)ApplicationStatus.Rejected,
                    include: q => q.Include(ja => ja.Worker));

                if (applicants != null && applicants.Any())
                {
                    foreach (var application in applicants)
                    {
                        if (application.Worker != null)
                        {
                            await _notificationService.CreateAsync(new CreateNotificationRequest
                            {
                                UserId = application.Worker.UserId,
                                Type = NotificationType.JobAcceptance,
                                Title = "Bài đăng công việc đã bị hủy",
                                Message = $"Bài đăng công việc \"{existingJobPost.Title}\" mà bạn đã ứng tuyển đã bị hủy bởi chủ tuyển dụng.",
                                RelatedEntityId = existingJobPost.Id
                            });
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.JobPostToJobPostDto(existingJobPost);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobPostDTO> UpdateJobPostUrgency(string id, bool isUrgent)
        {
            try
            {
                var existingJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == Guid.Parse(id),
                        include: q => q.Include(jp => jp.Farmer));
                if (existingJobPost == null)
                {
                    return null;
                }
                existingJobPost.IsUrgent = isUrgent;
                _unitOfWork.GetRepository<JobPost>().UpdateAsync(existingJobPost);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.JobPostToJobPostDto(existingJobPost);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobPostDTO> UpdateJobPostStatus(string id, JobPostStatus status)
        {
            try
            {
                if (!Guid.TryParse(id, out var jobPostId))
                {
                    throw new ArgumentException("Invalid job post id.");
                }

                if (!Enum.IsDefined(typeof(JobPostStatus), status))
                {
                    throw new Exception("Invalid status value");
                }

                var existingJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == jobPostId,
                        include: q => q.Include(jp => jp.Farmer));
                if (existingJobPost == null)
                {
                    return null;
                }

                var oldStatusId = existingJobPost.StatusId;
                var newStatusId = (int)status;
                var now = DateTime.UtcNow;

                existingJobPost.StatusId = newStatusId;
                existingJobPost.UpdatedAt = now;

                var farmer = existingJobPost.Farmer;
                if (farmer != null)
                {
                    var isFirstPublishedTransition =
                        oldStatusId != (int)JobPostStatus.Published &&
                        newStatusId == (int)JobPostStatus.Published;

                    var isFirstCompletedTransition =
                        oldStatusId != (int)JobPostStatus.Completed &&
                        newStatusId == (int)JobPostStatus.Completed;

                    if (isFirstPublishedTransition)
                    {
                        farmer.TotalJobsPosted += 1;
                        existingJobPost.PublishedAt = now;
                        await NotifyMatchingWorkersAsync(existingJobPost);
                    }

                    if (isFirstCompletedTransition)
                    {
                        farmer.TotalJobsCompleted += 1;
                    }

                    _unitOfWork.GetRepository<Farmer>().UpdateAsync(farmer);
                }

                _unitOfWork.GetRepository<JobPost>().UpdateAsync(existingJobPost);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.JobPostToJobPostDto(existingJobPost);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobPostDTO>> GetFilteredJobPosts(string? title, string? category, string? address, List<string?> skill, bool sortByDateDesc = true)
        {
            try
            {
                var jobPosts = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp =>
                            (string.IsNullOrEmpty(title) || jp.Title.Contains(title)) &&
                            (string.IsNullOrEmpty(category) || jp.JobCategory.Name == category) &&
                            (string.IsNullOrEmpty(address) || jp.Address.Contains(address)) &&
                            (skill == null || skill.Count == 0 || jp.JobSkillRequirements.Any(jsr => skill.Contains(jsr.Skill.Name))),
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobApplications)
                                .ThenInclude(ja => ja.Worker)
                                    .ThenInclude(w => w.User)
                            .Include(jp => jp.JobPostDays),
                        orderBy: jp => sortByDateDesc ? jp.OrderByDescending(x => x.CreatedAt) : jp.OrderBy(x => x.CreatedAt));
                if (jobPosts == null || !jobPosts.Any())
                {
                    return null;
                }
                var result = _mapper.JobPostsToJobPostDtos(jobPosts);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResponse<JobPostDTO>> GetFilteredJobPostsByFarmer(string? title, string? category, string? address, List<string?> skill, bool sortByDateDesc = true, int page = 1, int limit = 10)
        {
            try
            {
                page = page < 1 ? 1 : page;
                limit = limit < 1 ? 10 : limit;

                var currentUserId = GetCurrentUserId();
                var farmer = await _unitOfWork.GetRepository<Farmer>()
                    .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId);
                if (farmer == null)
                {
                    throw new UnauthorizedAccessException("No farmer profile created to view these job posts.");
                }

                var jobPosts = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp =>
                            jp.FarmerId == farmer.Id &&
                            (string.IsNullOrEmpty(title) || jp.Title.Contains(title)) &&
                            (string.IsNullOrEmpty(category) || jp.JobCategory.Name == category) &&
                            (string.IsNullOrEmpty(address) || jp.Address.Contains(address)) &&
                            (skill == null || skill.Count == 0 || jp.JobSkillRequirements.Any(jsr => skill.Contains(jsr.Skill.Name))),
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobApplications)
                                .ThenInclude(ja => ja.Worker)
                                    .ThenInclude(w => w.User)
                            .Include(jp => jp.JobPostDays),
                        orderBy: jp => sortByDateDesc ? jp.OrderByDescending(x => x.CreatedAt) : jp.OrderBy(x => x.CreatedAt));

                if (jobPosts == null || !jobPosts.Any())
                {
                    return new PaginatedResponse<JobPostDTO>
                    {
                        Data = new List<JobPostDTO>(),
                        Pagination = new PaginationMetadata
                        {
                            Page = page,
                            Limit = limit,
                            Total = 0,
                            TotalPages = 0
                        }
                    };
                }

                var total = jobPosts.Count;
                var pagedJobPosts = jobPosts
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToList();

                var result = _mapper.JobPostsToJobPostDtos(pagedJobPosts);

                return new PaginatedResponse<JobPostDTO>
                {
                    Data = result,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        Limit = limit,
                        Total = total,
                        TotalPages = (int)Math.Ceiling((double)total / limit)
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobPostDTO> SaveJobPostDraft(CreateJobPostRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == Guid.Empty)
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var farmer = await _unitOfWork.GetRepository<Farmer>()
                    .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId);

                if (farmer == null)
                {
                    throw new UnauthorizedAccessException("Only farmers can save job post drafts.");
                }

                var jobPost = _mapper.CreateJobPostRequestToJobPost(request);
                if (jobPost.Id == Guid.Empty)
                {
                    jobPost.Id = Guid.NewGuid();
                }

                jobPost.FarmerId = farmer.Id;
                jobPost.StatusId = (int)JobPostStatus.Draft;
                jobPost.CreatedAt = DateTime.UtcNow;
                jobPost.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.GetRepository<JobPost>().InsertAsync(jobPost);
                await _unitOfWork.SaveChangesAsync();

                if (request.SkillIds?.Any() == true)
                {
                    var requestedSkillIds = request.SkillIds
                        .Distinct()
                        .ToList();

                    var skills = requestedSkillIds.Any()
                        ? await _unitOfWork.GetRepository<Skill>()
                            .GetListAsync(predicate: s => requestedSkillIds.Contains(s.Id))
                        : new List<Skill>();

                    if (skills.Count == requestedSkillIds.Count)
                    {
                        var jobSkillRequirements = skills.Select(skill => new JobSkillRequirement
                        {
                            Id = Guid.NewGuid(),
                            JobPostId = jobPost.Id,
                            SkillId = skill.Id,
                            RequiredLevelId = (int)ProficiencyLevel.Beginner,
                            IsMandatory = true
                        }).ToList();

                        await _unitOfWork.GetRepository<JobSkillRequirement>().InsertRangeAsync(jobSkillRequirements);
                        await _unitOfWork.SaveChangesAsync();

                        _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                var createdJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == jobPost.Id,
                        include: q => q
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.Farmer));

                var result = _mapper.JobPostToJobPostDto(createdJobPost ?? jobPost);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobPostDTO>> GetFarmerDrafts()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == Guid.Empty)
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var farmer = await _unitOfWork.GetRepository<Farmer>()
                    .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId);

                if (farmer == null)
                {
                    throw new UnauthorizedAccessException("Only farmers can retrieve drafts.");
                }

                var drafts = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.FarmerId == farmer.Id && jp.StatusId == (int)JobPostStatus.Draft,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill),
                        orderBy: jp => jp.OrderByDescending(x => x.UpdatedAt));

                if (drafts == null || !drafts.Any())
                {
                    return new List<JobPostDTO>();
                }

                var result = _mapper.JobPostsToJobPostDtos(drafts);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedAdminJobPostsResponse> GetJobPostsForAdmin(int page = 1, int limit = 20)
        {
            try
            {
                page = page < 1 ? 1 : page;
                limit = limit < 1 ? 1 : limit;
                var skip = (page - 1) * limit;

                var jobPosts = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: null,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobDetails)
                            .ThenInclude(jd => jd.Worker),
                        orderBy: jp => jp.OrderByDescending(x => x.CreatedAt));

                var total = jobPosts?.Count ?? 0;
                var active = jobPosts?.Count(jp => jp.StatusId == (int)JobPostStatus.Published || jp.StatusId == (int)JobPostStatus.InProgress) ?? 0;
                var completed = jobPosts?.Count(jp => jp.StatusId == (int)JobPostStatus.Completed) ?? 0;
                var completionRate = total > 0 ? Math.Round((double)completed / total * 100, 1) : 0.0;

                var pageItems = jobPosts?.Skip(skip).Take(limit).Select(jp => new AdminJobPostItemDTO
                {
                    Id = jp.Id,
                    Title = jp.Title,
                    Farmer = jp.Farmer != null ? new SimpleUserDto { Id = jp.Farmer.Id, FullName = jp.Farmer.ContactName } : null,
                    Worker = jp.JobDetails?.FirstOrDefault()?.Worker != null ? new SimpleUserDto { Id = jp.JobDetails.First().Worker.Id, FullName = jp.JobDetails.First().Worker.FullName } : null,
                    Status = ((JobPostStatus)jp.StatusId).ToString(),
                    Salary = jp.WageAmount,
                    StartDate = jp.StartDate,
                    EndDate = jp.EndDate
                }).ToList() ?? new List<AdminJobPostItemDTO>();

                return new PaginatedAdminJobPostsResponse
                {
                    Data = pageItems,
                    Summary = new AdminJobPostSummaryDto
                    {
                        Total = total,
                        Active = active,
                        Completed = completed,
                        CompletionRate = completionRate
                    },
                    Total = total,
                    Page = page,
                    Limit = limit
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedJobDiscoveryResponse> SearchJobsAsync(JobSearchFilterRequest filter)
        {
            try
            {
                filter ??= new JobSearchFilterRequest();

                Worker? currentWorker = null;
                filter.PageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
                filter.PageSize = filter.PageSize < 1 ? 10 : Math.Min(filter.PageSize, 100); // Min 1, max 100 items per page
                var skip = (filter.PageNumber - 1) * filter.PageSize;

                var currentUserId = GetCurrentUserId();
                if (currentUserId != Guid.Empty)
                {
                    currentWorker = await _unitOfWork.GetRepository<Worker>()
                        .FirstOrDefaultAsync(predicate: w => w.UserId == currentUserId);

                    if (currentWorker != null)
                    {
                        if (!filter.MaxDistanceKm.HasValue || filter.MaxDistanceKm.Value <= 0)
                        {
                            filter.MaxDistanceKm = currentWorker.TravelRadiusKmPreference ?? 20;
                        }

                        var hasCoordinates =
                            filter.WorkerLatitude.HasValue &&
                            filter.WorkerLongitude.HasValue &&
                            (filter.WorkerLatitude.Value != 0 || filter.WorkerLongitude.Value != 0);

                        if (!hasCoordinates && !string.IsNullOrWhiteSpace(currentWorker.PrimaryLocation))
                        {
                            var primaryLocation = currentWorker.PrimaryLocation.Trim();
                            var locationFarm = await _unitOfWork.GetRepository<Farm>()
                                .FirstOrDefaultAsync(predicate:
                                    f => f.LocationName.ToLower() == primaryLocation.ToLower() ||
                                         f.Address.ToLower().Contains(primaryLocation.ToLower()));

                            if (locationFarm != null)
                            {
                                filter.WorkerLatitude = locationFarm.Latitude;
                                filter.WorkerLongitude = locationFarm.Longitude;
                            }
                        }
                    }
                }

                // Get all published and in-progress job posts with related data
                var query = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.StatusId == (int)JobPostStatus.Published,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobCategory)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobPostDays));

                if (query == null || !query.Any())
                {
                    return new PaginatedJobDiscoveryResponse
                    {
                        Jobs = new List<JobDiscoveryDTO>(),
                        TotalCount = 0,
                        PageNumber = filter.PageNumber,
                        PageSize = filter.PageSize
                    };
                }

                // Apply filters
                var filtered = JobDiscoveryHelper.ApplyJobFilters(query.ToList(), filter);

                // Fallback: if worker has a primary location but coordinates cannot be resolved,
                // default to that location name/address text filter.
                var hasFinalCoordinates =
                    filter.WorkerLatitude.HasValue &&
                    filter.WorkerLongitude.HasValue &&
                    (filter.WorkerLatitude.Value != 0 || filter.WorkerLongitude.Value != 0);

                if (!hasFinalCoordinates && currentWorker != null && !string.IsNullOrWhiteSpace(currentWorker.PrimaryLocation))
                {
                    var primaryLocation = currentWorker.PrimaryLocation.Trim();
                    filtered = filtered
                        .Where(jp => jp.Farm != null &&
                                     ((!string.IsNullOrWhiteSpace(jp.Farm.LocationName) && jp.Farm.LocationName.Contains(primaryLocation, StringComparison.OrdinalIgnoreCase)) ||
                                      (!string.IsNullOrWhiteSpace(jp.Farm.Address) && jp.Farm.Address.Contains(primaryLocation, StringComparison.OrdinalIgnoreCase))))
                        .ToList();
                }

                // Convert to DTOs using mapper
                var jobDtos = _mapper.JobPostsToJobDiscoveryDtos(filtered);

                // Post-process: Add distance, match score and other calculated fields
                foreach (var dto in jobDtos)
                {
                    JobDiscoveryHelper.ApplyDiscoveryCalculations(dto, filter);
                }

                // Apply sorting
                jobDtos = JobDiscoveryHelper.ApplyJobSorting(jobDtos, filter.SortBy);

                // Pagination
                var totalCount = jobDtos.Count;
                var paginatedJobs = jobDtos.Skip(skip).Take(filter.PageSize).ToList();

                return new PaginatedJobDiscoveryResponse
                {
                    Jobs = paginatedJobs,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    Message = $"Found {totalCount} job(s)"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching jobs: {ex.Message}");
            }
        }

        public async Task<List<JobDiscoveryDTO>> GetNearbyJobsAsync(decimal latitude, decimal longitude, double maxDistanceKm = 20)
        {
            try
            {
                var publishedJobs = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.StatusId == (int)JobPostStatus.Published,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobCategory)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobPostDays),
                        orderBy: jp => jp.OrderBy(x => x.StartDate));

                if (publishedJobs == null || !publishedJobs.Any())
                {
                    return new List<JobDiscoveryDTO>();
                }

                var nearbyJobs = new List<JobDiscoveryDTO>();

                foreach (var job in publishedJobs)
                {
                    if (job.Farm != null)
                    {
                        var distance = DistanceCalculator.GetDistanceInKilometers(
                            latitude, longitude,
                            job.Farm.Latitude, job.Farm.Longitude);

                        if (distance <= maxDistanceKm)
                        {
                            var dto = _mapper.JobPostToJobDiscoveryDto(job);
                            dto.DistanceKm = distance;
                            nearbyJobs.Add(dto);
                        }
                    }
                }

                // Sort by distance
                nearbyJobs = nearbyJobs.OrderBy(x => x.DistanceKm).ToList();
                return nearbyJobs;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting nearby jobs: {ex.Message}");
            }
        }

        public async Task<List<JobDiscoveryDTO>> GetJobsByDateAsync(string dateFilter)
        {
            try
            {
                var now = DateTime.UtcNow;
                DateOnly? dateStart = null;
                DateOnly? dateEnd = null;

                switch (dateFilter?.ToLower())
                {
                    case "today" or "hôm nay":
                        dateStart = DateOnly.FromDateTime(now.Date);
                        dateEnd = DateOnly.FromDateTime(now.Date);
                        break;
                    case "tomorrow" or "ngày mai":
                        dateStart = DateOnly.FromDateTime(now.Date.AddDays(1));
                        dateEnd = DateOnly.FromDateTime(now.Date.AddDays(1));
                        break;
                    case "weekend" or "cuối tuần":
                        // Friday evening to Sunday night
                        var dayOfWeek = (int)now.DayOfWeek;
                        var daysUntilFriday = (5 - dayOfWeek + 7) % 7;
                        if (daysUntilFriday == 0 && now.Hour >= 18) daysUntilFriday = 7;

                        dateStart = DateOnly.FromDateTime(now.Date.AddDays(daysUntilFriday));
                        dateEnd = DateOnly.FromDateTime(now.Date.AddDays(daysUntilFriday + 2));
                        break;
                    case "upcoming" or "sắp tới":
                        dateStart = DateOnly.FromDateTime(now.Date);
                        dateEnd = DateOnly.FromDateTime(now.Date.AddDays(30));
                        break;
                    default:
                        dateStart = DateOnly.FromDateTime(now.Date);
                        dateEnd = DateOnly.FromDateTime(now.Date.AddDays(7));
                        break;
                }

                var jobs = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.StatusId == (int)JobPostStatus.Published &&
                                       ((jp.StartDate >= dateStart && jp.StartDate <= dateEnd) ||
                                        (jp.JobPostDays != null && jp.JobPostDays.Any(d => d.WorkDate >= dateStart && d.WorkDate <= dateEnd))),
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobCategory)
                            .Include(jp => jp.JobPostDays)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill),
                        orderBy: jp => jp.OrderBy(x => x.StartDate));

                var result = jobs?.Select(j => _mapper.JobPostToJobDiscoveryDto(j)).ToList() ?? new List<JobDiscoveryDTO>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting jobs by date: {ex.Message}");
            }
        }

        public async Task<List<JobDiscoveryDTO>> GetJobsBySkillAsync(List<string> skills)
        {
            try
            {
                if (skills == null || !skills.Any())
                {
                    return new List<JobDiscoveryDTO>();
                }

                var jobs = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.StatusId == (int)JobPostStatus.Published &&
                                       jp.JobSkillRequirements.Any(jsr => skills.Contains(jsr.Skill.Name)),
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobCategory)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobPostDays),
                        orderBy: jp => jp.OrderBy(x => x.Title));

                var result = jobs?.Select(j => _mapper.JobPostToJobDiscoveryDto(j)).ToList() ?? new List<JobDiscoveryDTO>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting jobs by skill: {ex.Message}");
            }
        }

        public async Task<List<JobDiscoveryDTO>> GetJobsByWageRangeAsync(decimal minWage, decimal? maxWage = null)
        {
            try
            {
                if (minWage < 0 || (maxWage.HasValue && maxWage.Value < minWage))
                {
                    throw new ArgumentException("Invalid wage range");
                }

                var jobs = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.StatusId == (int)JobPostStatus.Published &&
                                       jp.WageAmount >= minWage &&
                                       (maxWage == null || jp.WageAmount <= maxWage),
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobCategory)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobPostDays),
                        orderBy: jp => jp.OrderByDescending(x => x.WageAmount));

                var result = jobs?.Select(j => _mapper.JobPostToJobDiscoveryDto(j)).ToList() ?? new List<JobDiscoveryDTO>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting jobs by wage range: {ex.Message}");
            }
        }

        public async Task<List<JobDiscoveryDTO>> GetJobsByTypeAsync(int jobTypeId)
        {
            try
            {
                if (jobTypeId <= 0)
                {
                    throw new ArgumentException("Invalid job type ID");
                }

                var jobs = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.StatusId == (int)JobPostStatus.Published && jp.JobTypeId == jobTypeId,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobCategory)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobPostDays),
                        orderBy: jp => jp.OrderBy(x => x.Title));

                var result = jobs?.Select(j => _mapper.JobPostToJobDiscoveryDto(j)).ToList() ?? new List<JobDiscoveryDTO>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting jobs by type: {ex.Message}");
            }
        }

        public async Task<List<JobDiscoveryDTO>> GetUrgentJobsAsync(decimal latitude, decimal longitude, double maxDistanceKm = 20)
        {
            try
            {
                var urgentJobs = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.StatusId == (int)JobPostStatus.Published && jp.IsUrgent,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobCategory)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(jp => jp.JobPostDays),
                        orderBy: jp => jp.OrderBy(x => x.StartDate));

                if (urgentJobs == null || !urgentJobs.Any())
                {
                    return new List<JobDiscoveryDTO>();
                }

                var nearby = new List<JobDiscoveryDTO>();
                foreach (var job in urgentJobs)
                {
                    if (job.Farm != null)
                    {
                        var distance = DistanceCalculator.GetDistanceInKilometers(
                            latitude, longitude,
                            job.Farm.Latitude, job.Farm.Longitude);

                        if (distance <= maxDistanceKm)
                        {
                            var dto = _mapper.JobPostToJobDiscoveryDto(job);
                            dto.DistanceKm = distance;
                            nearby.Add(dto);
                        }
                    }
                }

                nearby = nearby.OrderBy(x => x.DistanceKm).ToList();
                return nearby;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting urgent jobs: {ex.Message}");
            }
        }

        private static int ResolveBillableDays(DateOnly? startDate, DateOnly? endDate, IEnumerable<DateOnly>? dayDates)
        {
            var selectedDayCount = dayDates?.Distinct().Count() ?? 0;
            if (selectedDayCount > 0)
            {
                return selectedDayCount;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                var spanDays = endDate.Value.DayNumber - startDate.Value.DayNumber + 1;
                return Math.Max(1, spanDays);
            }

            return 1;
        }

        private async Task NotifyMatchingWorkersAsync(JobPost jobPost)
        {
            var jobSkillIds = await _unitOfWork.GetRepository<JobSkillRequirement>()
                .GetListAsync(
                    predicate: jsr => jsr.JobPostId == jobPost.Id,
                    include: null,
                    orderBy: null);

            if (jobSkillIds == null || !jobSkillIds.Any())
                return;

            var requiredSkillIds = jobSkillIds.Select(jsr => jsr.SkillId).ToHashSet();

            var matchingWorkerSkills = await _unitOfWork.GetRepository<WorkerSkill>()
                .GetListAsync(
                    predicate: ws => requiredSkillIds.Contains(ws.SkillId),
                    include: q => q.Include(ws => ws.Worker),
                    orderBy: null);

            if (matchingWorkerSkills == null || !matchingWorkerSkills.Any())
                return;

            var distinctWorkers = matchingWorkerSkills
                .Where(ws => ws.Worker != null)
                .GroupBy(ws => ws.WorkerId)
                .Select(g => g.First().Worker)
                .ToList();

            foreach (var worker in distinctWorkers)
            {
                await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    UserId = worker.UserId,
                    Type = NotificationType.NearbyJobOpening,
                    Title = "Công việc mới phù hợp với kỹ năng của bạn",
                    Message = $"Một công việc mới \"{jobPost.Title}\" vừa được đăng và phù hợp với kỹ năng của bạn. Hãy xem ngay!",
                    RelatedEntityId = jobPost.Id
                });
            }
        }

        public async Task<List<WorkersPerDayDTO>> GetAcceptedWorkersPerDayAsync(Guid jobPostId)
        {
            try
            {
                var jobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == jobPostId,
                        include: q => q.Include(jp => jp.JobPostDays));

                if (jobPost == null)
                    throw new KeyNotFoundException($"Job post with id '{jobPostId}' was not found.");

                var acceptedApplications = await _unitOfWork.GetRepository<JobApplication>()
                    .GetListAsync(
                        predicate: ja =>
                            ja.JobPostId == jobPostId &&
                            ja.StatusId == (int)ApplicationStatus.Accepted,
                        include: q => q
                            .Include(ja => ja.Worker)
                                .ThenInclude(w => w.User));

                var result = jobPost.JobPostDays
                    .OrderBy(day => day.WorkDate)
                    .Select(day =>
                    {
                        var workersOnDay = acceptedApplications
                            .Where(ja => ja.WorkDates != null &&
                                         ja.WorkDates.Any(dt => DateOnly.FromDateTime(dt) == day.WorkDate))
                            .Select(ja => new WorkerSummaryDTO
                            {
                                WorkerId   = ja.Worker.Id,
                                FullName   = ja.Worker.FullName,
                                PhoneNumber = ja.Worker.User?.PhoneNumber ?? string.Empty,
                                AvatarUrl  = ja.Worker.AvatarUrl
                            })
                            .ToList();

                        return new WorkersPerDayDTO
                        {
                            Date = day.WorkDate,
                            AcceptedWorkerCount = workersOnDay.Count,
                            Workers = workersOnDay
                        };
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SavedJobPostDTO> ToggleSaveJobPostAsync(Guid jobPostId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == Guid.Empty)
                    throw new UnauthorizedAccessException("User is not authenticated.");

                var worker = await _unitOfWork.GetRepository<Worker>()
                    .FirstOrDefaultAsync(predicate: w => w.UserId == currentUserId);
                if (worker == null)
                    throw new UnauthorizedAccessException("Only workers can save job posts.");

                var jobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == jobPostId,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill));
                if (jobPost == null)
                    throw new KeyNotFoundException($"Job post with id '{jobPostId}' was not found.");

                var existing = await _unitOfWork.GetRepository<SavedJobPost>()
                    .FirstOrDefaultAsync(predicate: s => s.WorkerId == worker.Id && s.JobPostId == jobPostId);

                if (existing != null)
                {
                    _unitOfWork.GetRepository<SavedJobPost>().DeleteAsync(existing);
                    await _unitOfWork.SaveChangesAsync();
                    return null;
                }

                var saved = new SavedJobPost
                {
                    Id = Guid.NewGuid(),
                    WorkerId = worker.Id,
                    JobPostId = jobPostId,
                    SavedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<SavedJobPost>().InsertAsync(saved);
                await _unitOfWork.SaveChangesAsync();

                return new SavedJobPostDTO
                {
                    Id = saved.Id,
                    WorkerId = saved.WorkerId,
                    SavedAt = saved.SavedAt,
                    JobPost = _mapper.JobPostToJobPostDto(jobPost)
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<SavedJobPostDTO>> GetSavedJobPostsAsync()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == Guid.Empty)
                    throw new UnauthorizedAccessException("User is not authenticated.");

                var worker = await _unitOfWork.GetRepository<Worker>()
                    .FirstOrDefaultAsync(predicate: w => w.UserId == currentUserId);
                if (worker == null)
                    throw new UnauthorizedAccessException("Only workers can retrieve saved job posts.");

                var savedPosts = await _unitOfWork.GetRepository<SavedJobPost>()
                    .GetListAsync(
                        predicate: s => s.WorkerId == worker.Id,
                        include: q => q
                            .Include(s => s.JobPost)
                            .ThenInclude(jp => jp.Farmer)
                            .Include(s => s.JobPost)
                            .ThenInclude(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill)
                            .Include(s => s.JobPost)
                            .ThenInclude(jp => jp.JobPostDays),
                        orderBy: s => s.OrderByDescending(x => x.SavedAt));

                return savedPosts.Select(s => new SavedJobPostDTO
                {
                    Id = s.Id,
                    WorkerId = s.WorkerId,
                    SavedAt = s.SavedAt,
                    JobPost = _mapper.JobPostToJobPostDto(s.JobPost)
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}