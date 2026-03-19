using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
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

                var requestedSkillIds = request.JobSkillRequirementIds
                    .Where(skillId => skillId != Guid.Empty)
                    .Distinct()
                    .ToList();

                var skills = requestedSkillIds.Any()
                    ? await _unitOfWork.GetRepository<Skill>()
                        .GetListAsync(predicate: s => requestedSkillIds.Contains(s.Id))
                    : new List<Skill>();

                if (skills.Count != requestedSkillIds.Count)
                {
                    var foundSkillIds = skills.Select(s => s.Id).ToHashSet();
                    var invalidSkillIds = requestedSkillIds.Where(id => !foundSkillIds.Contains(id));
                    throw new ArgumentException($"Invalid skill id(s): {string.Join(", ", invalidSkillIds)}");
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

        public async Task<JobPostDTO> UpdateJobPost(Guid id,UpdateJobPostRequest request)
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

        public async Task<JobPostDTO> UpdateJobPostStatus(string id, string status)
        {
            try
            {
                var existingJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == Guid.Parse(id));
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
    }
}
