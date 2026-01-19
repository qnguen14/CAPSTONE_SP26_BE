using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
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
                        include: null,
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
                        include: null);
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
                var jobPost = _mapper.CreateJobPostRequestToJobPost(request);

                await _unitOfWork.GetRepository<JobPost>().InsertAsync(jobPost);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.JobPostToJobPostDto(jobPost);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobPostDTO> UpdateJobPost(UpdateJobPostRequest request)
        {
            try
            {
                var existingJobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(
                        predicate: jp => jp.Id == request.Id);

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
    }
}
