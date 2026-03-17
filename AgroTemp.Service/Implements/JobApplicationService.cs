using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AgroTemp.Service.Implements
{
    public class JobApplicationService : BaseService<JobApplication>, IJobApplicationService
    {
        public JobApplicationService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<JobApplicationDTO>> GetAllJobApplications()
        {
            try
            {
                var jobApplications = await _unitOfWork.GetRepository<JobApplication>()
                    .GetListAsync(
                        predicate: null,
                        include: null,
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
                        include: null);

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
                var jobApplication = _mapper.CreateJobApplicationRequestToJobApplication(request);

                // set defaults
                jobApplication.Id = Guid.NewGuid();
                jobApplication.AppliedAt = DateTime.UtcNow;
                jobApplication.StatusId = (int)ApplicationStatus.Pending;

                await _unitOfWork.GetRepository<JobApplication>().InsertAsync(jobApplication);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.JobApplicationToJobApplicationDto(jobApplication);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobApplicationDTO> UpdateJobApplication(UpdateJobApplicationRequest request)
        {
            try
            {
                var existingJobApplication = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(
                        predicate: ja => ja.Id == request.Id);

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
                        predicate: ja => ja.Id == guid);

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
    }
}
