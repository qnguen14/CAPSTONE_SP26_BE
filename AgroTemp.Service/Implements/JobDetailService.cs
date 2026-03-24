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
        private readonly IWalletTransactionService _walletTransactionService;

        public JobDetailService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper,
            IWalletTransactionService walletTransactionService) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _walletTransactionService = walletTransactionService;
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

        /////////////////////////////////////////////////////
        public async Task<JobDetailResponseDTO> ReportDailyWork(CreateDailyReportRequest request)
        {
            try
            {
                var id = request.JobApplicationId;
                // Check if application exists and is accepted
                var application = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(ja => ja.Id.ToString() == id.ToString(),null,null);
                if (application == null || application.Status != ApplicationStatus.Accepted)
                {
                    throw new Exception("Job application not found or not accepted");
                }

                // Check if already reported today
                var today = DateTime.UtcNow.Date;
                var existingReport = await _unitOfWork.GetRepository<JobDetail>()
                    .FirstOrDefaultAsync(jd => jd.JobApplicationId == request.JobApplicationId && jd.WorkDate == today,null,null);
                if (existingReport != null)
                {
                    throw new Exception("Already reported for today");
                }
                var jobPost = await _unitOfWork.GetRepository<JobPost>().FirstOrDefaultAsync(jp => jp.Id == application.JobPostId, null, null);

                // Create new JobDetail
                var jobDetail = new JobDetail
                {
                    Id = Guid.NewGuid(),
                    JobApplicationId = request.JobApplicationId,
                    JobPostId = application.JobPostId,
                    WorkerId = application.WorkerId,
                    StatusId = (int)JobStatus.Reported,
                    JobPrice = jobPost.WageAmount,
                    WorkDate = today,
                    WorkerDescription = request.WorkerDescription,
                    CreatedAt = DateTime.UtcNow
                };

                // JobAttachment is temporarily disabled because the current database does not have this table yet.

                await _unitOfWork.GetRepository<JobDetail>().InsertAsync(jobDetail);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.JobDetailToJobDetailResponseDto(jobDetail);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobDetailResponseDTO>> GetJobDetailsByWorkerId(Guid workerId)
        {
            try
            {
                var jobDetails = await _unitOfWork.GetRepository<JobDetail>()
                    .GetListAsync(
                        predicate: jd => jd.WorkerId == workerId,
                        include: null,
                        orderBy: jd => jd.OrderByDescending(x => x.CreatedAt));
                if (jobDetails == null || !jobDetails.Any())
                {
                    return new List<JobDetailResponseDTO>();
                }
                var result = _mapper.JobDetailsToJobDetailResponseDtos(jobDetails);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobDetailResponseDTO>> GetJobDetailsByJobPostId(Guid jobPostId)
        {
            try
            {
                var jobDetails = await _unitOfWork.GetRepository<JobDetail>()
                    .GetListAsync(
                        predicate: jd => jd.JobPostId == jobPostId,
                        include: null,
                        orderBy: jd => jd.OrderByDescending(x => x.CreatedAt));
                if (jobDetails == null || !jobDetails.Any())
                {
                    return new List<JobDetailResponseDTO>();
                }
                var result = _mapper.JobDetailsToJobDetailResponseDtos(jobDetails);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobDetailResponseDTO> ApproveJobDetail(Guid id, ApproveJobDetailRequest request)
        {
            try
            {
                if (request.FarmerApprovedPercent < 0 || request.FarmerApprovedPercent > 100)
                {
                    throw new Exception("Farmer approved percent must be between 0 and 100");
                }

                var jobDetail = await _unitOfWork.GetRepository<JobDetail>()
                    .FirstOrDefaultAsync(jd => jd.Id == id,null,null);
                if (jobDetail == null)
                {
                    throw new Exception("Job detail not found");
                }

                if (jobDetail.Status != JobStatus.Reported)
                {
                    throw new Exception("Job detail is not in reported status");
                }

                // Calculate payments
                var workerPayment = jobDetail.JobPrice * request.FarmerApprovedPercent / 100;
                var refund = jobDetail.JobPrice - workerPayment;

                jobDetail.FarmerApprovedPercent = request.FarmerApprovedPercent;
                jobDetail.FarmerFeedback = request.FarmerFeedback;
                jobDetail.WorkerPaymentAmount = workerPayment;
                jobDetail.RefundAmount = refund;
                jobDetail.Status = JobStatus.Completed;
                jobDetail.CompletedAt = DateTime.UtcNow;
                jobDetail.UpdatedAt = DateTime.UtcNow;

                await _walletTransactionService.ApplyJobSettlementAsync(jobDetail, workerPayment, refund);

                _unitOfWork.GetRepository<JobDetail>().UpdateAsync(jobDetail);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.JobDetailToJobDetailResponseDto(jobDetail);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
   
         public async Task<JobDetailResponseDTO> GetById(string id)
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
                var result = _mapper.JobDetailToJobDetailResponseDto(jobDetail);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
