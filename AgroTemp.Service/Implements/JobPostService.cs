using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Helpers;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
                            .ThenInclude(jsr => jsr.Skill),
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
                            .ThenInclude(jsr => jsr.Skill));
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
                            .ThenInclude(jsr => jsr.Skill),
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
                            .ThenInclude(jsr => jsr.Skill),
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
                    throw new UnauthorizedAccessException("Only farmers can create job posts.");
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

                var jobPost = _mapper.CreateJobPostRequestToJobPost(request);
                if (jobPost.Id == Guid.Empty)
                {
                    jobPost.Id = Guid.NewGuid();
                }
                jobPost.FarmerId = farmer.Id;
                jobPost.StatusId = (int)JobPostStatus.Draft;
                jobPost.CreatedAt = DateTime.UtcNow;
                jobPost.UpdatedAt = DateTime.UtcNow;
                jobPost.PublishedAt = default;

                var totalDays = (request.StartDate.HasValue && request.EndDate.HasValue)
                    ? (request.EndDate.Value.ToDateTime(TimeOnly.MinValue) - request.StartDate.Value.ToDateTime(TimeOnly.MinValue)).TotalDays
                    : 0;
                var lockAmount = request.JobTypeId == JobType.PerJob
                    ? request.WageAmount
                    : request.WageAmount * request.WorkersNeeded * (decimal)totalDays;
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
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill));

                var result = _mapper.JobPostToJobPostDto(createdJobPost ?? jobPost);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobPostDTO> UpdateJobPost(Guid id, UpdateJobPostRequest request)
        {
            try
            {
                var existingJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == id,
                        include: q => q.Include(jp => jp.JobSkillRequirements).ThenInclude(jsr => jsr.Skill));

                if (existingJobPost == null)
                {
                    return null;
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

                _mapper.UpdateJobPostRequestToJobPost(request, existingJobPost);
                _unitOfWork.GetRepository<JobPost>().UpdateAsync(existingJobPost);
                await _unitOfWork.SaveChangesAsync();
                
                var updatedJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == id,
                        include: q => q.Include(jp => jp.JobSkillRequirements).ThenInclude(jsr => jsr.Skill));
                
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
                            .ThenInclude(jsr => jsr.Skill),
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

        public async Task<List<JobPostDTO>> GetFilteredJobPostsByFarmer(string? title, string? category, string? address, List<string?> skill, bool sortByDateDesc = true)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var farmer = await _unitOfWork.GetRepository<Farmer>()
                    .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId);
                if (farmer == null) {
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
                            .ThenInclude(jsr => jsr.Skill),
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

        public async Task<PaginatedJobDiscoveryResponse> SearchJobsAsync(JobSearchFilterRequest filter)
        {
            try
            {
                filter.PageSize = Math.Min(filter.PageSize, 100); // Max 100 items per page
                var skip = (filter.PageNumber - 1) * filter.PageSize;

                // Get all published and active job posts with related data
                var query = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp => jp.StatusId == (int)JobPostStatus.Published,
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobCategory)
                            .Include(jp => jp.JobSkillRequirements)
                            .ThenInclude(jsr => jsr.Skill));

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
                            .ThenInclude(jsr => jsr.Skill),
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
                                        (jp.SelectedDays != null && jp.SelectedDays.Any(d => d >= dateStart && d <= dateEnd))),
                        include: q => q
                            .Include(jp => jp.Farmer)
                            .Include(jp => jp.Farm)
                            .Include(jp => jp.JobCategory)
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
                            .ThenInclude(jsr => jsr.Skill),
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
                            .ThenInclude(jsr => jsr.Skill),
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
                            .ThenInclude(jsr => jsr.Skill),
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
                            .ThenInclude(jsr => jsr.Skill),
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
    }
}