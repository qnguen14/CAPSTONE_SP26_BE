using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AgroTemp.Service.Implements
{
    public class JobCategoryService : BaseService<JobCategory>, IJobCategoryService
    {
        private readonly IMapperlyMapper _mapper;

        public JobCategoryService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<JobCategoryDTO>> GetAllJobCategories()
        {
            try
            {
                var jobCategories = await _unitOfWork.GetRepository<JobCategory>()
                    .GetListAsync(
                        predicate: null,
                        include: null,
                        orderBy: jc => jc.OrderBy(x => x.Name));
                if (jobCategories == null || !jobCategories.Any())
                {
                    return null;
                }
                var result = _mapper.JobCategoriesToJobCategoryDtos(jobCategories);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobCategoryDTO> GetJobCategoryById(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var jobCategory = await _unitOfWork.GetRepository<JobCategory>()
                    .FirstOrDefaultAsync(
                        predicate: jc => jc.Id == guid,
                        include: null);
                if (jobCategory == null)
                {
                    return null;
                }
                var result = _mapper.JobCategoryToJobCategoryDto(jobCategory);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobCategoryDTO> CreateJobCategory(CreateJobCategoryRequest request)
        {
            try
            {
                var jobCategory = _mapper.CreateJobCategoryRequestToJobCategory(request);

                await _unitOfWork.GetRepository<JobCategory>().InsertAsync(jobCategory);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.JobCategoryToJobCategoryDto(jobCategory);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobCategoryDTO> UpdateJobCategory(Guid id, UpdateJobCategoryRequest request)
        {
            try
            {
                var existingJobCategory = await _unitOfWork.GetRepository<JobCategory>()
                    .FirstOrDefaultAsync(
                        predicate: jc => jc.Id == id,
                        include: null);

                if (existingJobCategory == null)
                {
                    return null;
                }

                _mapper.UpdateJobCategoryRequestToJobCategory(request, existingJobCategory);
                _unitOfWork.GetRepository<JobCategory>().UpdateAsync(existingJobCategory);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.JobCategoryToJobCategoryDto(existingJobCategory);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteJobCategory(string id)
        {
            try
            {
                var guid = Guid.Parse(id);

                var existingJobCategory = await _unitOfWork.GetRepository<JobCategory>()
                    .FirstOrDefaultAsync(
                        predicate: jc => jc.Id == guid,
                        include: null);

                if (existingJobCategory == null)
                {
                    return false;
                }

                _unitOfWork.GetRepository<JobCategory>().DeleteAsync(existingJobCategory);
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
