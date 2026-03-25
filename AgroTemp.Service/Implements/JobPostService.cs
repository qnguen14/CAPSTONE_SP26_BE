using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobPost;
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
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Service.Implements
{
    public class JobPostService : BaseService<JobPost>, IJobPostService
    {
        private readonly IMapperlyMapper _mapper;

        public JobPostService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
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

                var requestedSkillNames = request.JobSkillRequirements?
                    .Select(jsr => jsr.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Distinct()
                    .ToList() ?? new List<string>();

                var skills = requestedSkillNames.Any()
                    ? await _unitOfWork.GetRepository<Skill>()
                        .GetListAsync(predicate: s => requestedSkillNames.Contains(s.Name))
                    : new List<Skill>();

                if (skills.Count != requestedSkillNames.Count)
                {
                    var foundSkillNames = skills.Select(s => s.Name).ToHashSet();
                    var invalidSkillNames = requestedSkillNames.Where(name => !foundSkillNames.Contains(name));
                    throw new ArgumentException($"Invalid skill name(s): {string.Join(", ", invalidSkillNames)}");
                }

                var jobPost = _mapper.CreateJobPostRequestToJobPost(request);
                if (jobPost.Id == Guid.Empty)
                {
                    jobPost.Id = Guid.NewGuid();
                }
                jobPost.FarmerId = farmer.Id;

                if (skills.Any())
                {
                    jobPost.RequiredSkills = string.Join(", ", skills.Select(skill => skill.Name));
                }

                await _unitOfWork.GetRepository<JobPost>().InsertAsync(jobPost);
                await _unitOfWork.SaveChangesAsync();

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
                    await _unitOfWork.SaveChangesAsync();
                }

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
                        predicate: jp => jp.Id == id);

                if (existingJobPost == null)
                {
                    return null;
                }

                _mapper.UpdateJobPostRequestToJobPost(request, existingJobPost);
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

        public async Task<JobPostDTO> UpdateJobPostStatus(string id, string status)
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

                if (Enum.TryParse(status, out JobPostStatus jobPostStatus))
                {
                    existingJobPost.StatusId = (int)jobPostStatus;
                }
                else
                {
                    throw new Exception("Invalid status value");
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

        public async Task<List<JobPostDTO>> GetFilteredJobPosts(string? title, string? category, string? address, string? skill)
        {
            try
            {
                var jobPosts = await _unitOfWork.GetRepository<JobPost>()
                    .GetListAsync(
                        predicate: jp =>
                            (string.IsNullOrEmpty(title) || jp.Title.Contains(title)) &&
                            (string.IsNullOrEmpty(category) || jp.JobCategory.Name == category) &&
                            (string.IsNullOrEmpty(address) || jp.Address.Contains(address)) &&
                            (string.IsNullOrEmpty(skill) || jp.RequiredSkills.Contains(skill)),
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

                if (request.JobSkillRequirements?.Any() == true)
                {
                    var requestedSkillNames = request.JobSkillRequirements
                        .Select(jsr => jsr.Name)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .Distinct()
                        .ToList();

                    var skills = requestedSkillNames.Any()
                        ? await _unitOfWork.GetRepository<Skill>()
                            .GetListAsync(predicate: s => requestedSkillNames.Contains(s.Name))
                        : new List<Skill>();

                    if (skills.Count == requestedSkillNames.Count)
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
                        
                        jobPost.RequiredSkills = string.Join(", ", skills.Select(skill => skill.Name));
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
                DateTime? dateStart = null;
                DateTime? dateEnd = null;

                switch (dateFilter?.ToLower())
                {
                    case "today":
                        dateStart = now.Date;
                        dateEnd = now.Date.AddDays(1).AddTicks(-1);
                        break;
                    case "tomorrow":
                        dateStart = now.Date.AddDays(1);
                        dateEnd = now.Date.AddDays(2).AddTicks(-1);
                        break;
                    case "weekend":
                        // Friday evening to Sunday night
                        var dayOfWeek = (int)now.DayOfWeek;
                        var daysUntilFriday = (5 - dayOfWeek + 7) % 7;
                        if (daysUntilFriday == 0 && now.Hour >= 18) daysUntilFriday = 7;
                        
                        dateStart = now.Date.AddDays(daysUntilFriday);
                        dateEnd = now.Date.AddDays(daysUntilFriday + 3).AddTicks(-1);
                        break;
                    case "upcoming":
                        dateStart = now.Date;
                        dateEnd = now.Date.AddDays(30).AddTicks(-1);
                        break;
                    default:
                        dateStart = now.Date;
                        dateEnd = now.Date.AddDays(7).AddTicks(-1);
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
                                       skills.Any(s => jp.RequiredSkills.Contains(s)),
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
    }
}