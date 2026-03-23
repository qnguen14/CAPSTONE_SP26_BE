using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobDetail;
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
    public class JobDetailService : BaseService<JobDetail>, IJobDetailService
    {
        private readonly IMapperlyMapper _mapper;

        public JobDetailService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<JobDetailDTO>> GetAllJobDetails()
        {
            try
            {
                var jobDetails = await _unitOfWork.GetRepository<JobDetail>()
                    .GetListAsync(
                        predicate: null,
                        include: null,
                        orderBy: jd => jd.OrderBy(x => x.CreatedAt));
                if (jobDetails == null || !jobDetails.Any())
                {
                    return null;
                }
                var result = _mapper.JobDetailsToJobDetailDtos(jobDetails);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobDetailDTO> GetJobDetailById(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var jobDetail = await _unitOfWork.GetRepository<JobDetail>()
                    .FirstOrDefaultAsync(
                        predicate: jd => jd.Id == guid,
                        include: null);
                if (jobDetail == null)
                {
                    return null;
                }
                var result = _mapper.JobDetailToJobDetailDto(jobDetail);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobDetailDTO> CreateJobDetail(CreateJobDetailRequest request)
        {
            try
            {
                var jobDetail = _mapper.CreateJobDetailRequestToJobDetail(request);
                jobDetail.CreatedAt = DateTime.UtcNow;
                jobDetail.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.GetRepository<JobDetail>().InsertAsync(jobDetail);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.JobDetailToJobDetailDto(jobDetail);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobDetailDTO> UpdateJobDetail(Guid id, UpdateJobDetailRequest request)
        {
            try
            {
                var existingJobDetail = await _unitOfWork.GetRepository<JobDetail>()
                    .FirstOrDefaultAsync(
                        predicate: jd => jd.Id == id);

                if (existingJobDetail == null)
                {
                    return null;
                }

                _mapper.UpdateJobDetailRequestToJobDetail(request, existingJobDetail);
                existingJobDetail.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<JobDetail>().UpdateAsync(existingJobDetail);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.JobDetailToJobDetailDto(existingJobDetail);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteJobDetail(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var existingJobDetail = await _unitOfWork.GetRepository<JobDetail>()
                    .FirstOrDefaultAsync(
                        predicate: jd => jd.Id == guid);
                if (existingJobDetail == null)
                {
                    return false;
                }
                _unitOfWork.GetRepository<JobDetail>().DeleteAsync(existingJobDetail);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobDetailDTO> UpdateJobDetailStatus(string id, string status)
        {
            try
            {
                var existingJobDetail = await _unitOfWork.GetRepository<JobDetail>()
                    .FirstOrDefaultAsync(
                        predicate: jd => jd.Id == Guid.Parse(id));
                if (existingJobDetail == null)
                {
                    return null;
                }

                if (Enum.TryParse(status, out JobStatus jobStatus))
                {
                    existingJobDetail.StatusId = (int)jobStatus;
                }
                else
                {
                    throw new Exception("Invalid status value");
                }

                existingJobDetail.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<JobDetail>().UpdateAsync(existingJobDetail);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.JobDetailToJobDetailDto(existingJobDetail);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
