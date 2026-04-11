using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobDetail;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Domain.Metadata;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Service.Implements
{
    public class JobDetailService : BaseService<JobDetail>, IJobDetailService
    {
        private readonly IMapperlyMapper _mapper;
        private readonly IWalletService _walletService;

        public JobDetailService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper,
            IWalletService walletService) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _walletService = walletService;
        }

        public async Task<List<JobDetailDTO>> GetAllJobDetails()
        {
            try
            {
                var jobDetails = await _unitOfWork.GetRepository<JobDetail>()
                    .GetListAsync(
                        predicate: null,
                        include: jd => jd.Include(x => x.Worker).ThenInclude(w => w.User),
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
                        include: jd => jd.Include(x => x.Worker).ThenInclude(w => w.User));
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

                var oldStatusId = existingJobDetail.StatusId;

                if (Enum.TryParse(status, out JobStatus jobStatus))
                {
                    existingJobDetail.StatusId = (int)jobStatus;

                    if (oldStatusId != (int)JobStatus.Completed &&
                        existingJobDetail.StatusId == (int)JobStatus.Completed)
                    {
                        var worker = await _unitOfWork.GetRepository<Worker>()
                            .FirstOrDefaultAsync(predicate: w => w.Id == existingJobDetail.WorkerId);
                        
                        if (worker != null)
                        {
                            worker.TotalJobsCompleted += 1;
                            worker.UpdatedAt = DateTime.UtcNow;
                            _unitOfWork.GetRepository<Worker>().UpdateAsync(worker);
                        }
                    }
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

        public async Task<JobDetailResponseDTO> ReportDailyWork(Guid id, CreateDailyReportRequest request)
        {
            try
            {
                var application = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(ja => ja.Id.ToString() == id.ToString(),null,null);
                if (application == null || application.Status != ApplicationStatus.Accepted)
                {
                    throw new Exception("Job application not found or not accepted");
                }

                var today = DateTime.UtcNow.Date;
                var existingReport = await _unitOfWork.GetRepository<JobDetail>()
                    .FirstOrDefaultAsync(jd => jd.JobApplicationId == id && jd.WorkDate == today,null,null);
                if (existingReport != null)
                {
                    throw new Exception("Already reported for today");
                }
                var jobPost = await _unitOfWork.GetRepository<JobPost>().FirstOrDefaultAsync(jp => jp.Id == application.JobPostId, null, null);

                var jobDetail = new JobDetail
                {
                    Id = Guid.NewGuid(),
                    JobApplicationId = id,
                    JobPostId = application.JobPostId,
                    WorkerId = application.WorkerId,
                    StatusId = (int)JobStatus.Reported,
                    JobPrice = jobPost.WageAmount,
                    WorkDate = today,
                    WorkerDescription = request.WorkerDescription,
                    CreatedAt = DateTime.UtcNow
                };

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

        public async Task<PaginatedResponse<JobDetailResponseDTO>> GetJobDetailsByWorkerId(Guid workerId, int page = 1, int limit = 10)
        {
            try
            {
                var skip = (page - 1) * limit;
                var predicate = (System.Linq.Expressions.Expression<Func<JobDetail, bool>>)(jd => jd.WorkerId == workerId);

                var total = await _unitOfWork.GetRepository<JobDetail>().CountAsync(predicate);

                var query = _unitOfWork.GetRepository<JobDetail>().CreateBaseQuery(
                    predicate: predicate,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    include: q => q.Include(x => x.Worker).ThenInclude(w => w.User));

                var items = await query.Skip(skip).Take(limit).ToListAsync();
                var data = _mapper.JobDetailsToJobDetailResponseDtos(items);

                return new PaginatedResponse<JobDetailResponseDTO>
                {
                    Data = data,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        Limit = limit,
                        Total = total,
                        TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit)
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResponse<JobDetailResponseDTO>> GetJobDetailsByJobPostId(Guid jobPostId, int page = 1, int limit = 10)
        {
            try
            {
                var skip = (page - 1) * limit;
                var predicate = (System.Linq.Expressions.Expression<Func<JobDetail, bool>>)(jd => jd.JobPostId == jobPostId);

                var total = await _unitOfWork.GetRepository<JobDetail>().CountAsync(predicate);

                var query = _unitOfWork.GetRepository<JobDetail>().CreateBaseQuery(
                    predicate: predicate,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    include: q => q.Include(x => x.Worker).ThenInclude(w => w.User));

                var items = await query.Skip(skip).Take(limit).ToListAsync();
                var data = _mapper.JobDetailsToJobDetailResponseDtos(items);

                return new PaginatedResponse<JobDetailResponseDTO>
                {
                    Data = data,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        Limit = limit,
                        Total = total,
                        TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit)
                    }
                };
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

                var workerPayment = jobDetail.JobPrice * request.FarmerApprovedPercent / 100;
                var refund = jobDetail.JobPrice - workerPayment;

                var worker = await _unitOfWork.GetRepository<Worker>()
                    .FirstOrDefaultAsync(predicate: w => w.Id == jobDetail.WorkerId);

                if(worker != null)
                {
                    worker.TotalJobsCompleted += 1;
                    _unitOfWork.GetRepository<Worker>().UpdateAsync(worker);
                }

                jobDetail.FarmerApprovedPercent = request.FarmerApprovedPercent;
                jobDetail.FarmerFeedback = request.FarmerFeedback;
                jobDetail.WorkerPaymentAmount = workerPayment;
                jobDetail.RefundAmount = refund;
                jobDetail.Status = JobStatus.Completed;
                jobDetail.CompletedAt = DateTime.UtcNow;
                jobDetail.UpdatedAt = DateTime.UtcNow;

                await _walletService.ApplyJobSettlementAsync(jobDetail, workerPayment, refund);

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
   
        //  public async Task<JobDetailResponseDTO> GetById(string id)
        // {
        //     try
        //     {
        //         var guid = Guid.Parse(id);
        //         var jobDetail = await _unitOfWork.GetRepository<JobDetail>()
        //             .FirstOrDefaultAsync(
        //                 predicate: jd => jd.Id == guid,
        //                 include: null);
        //         if (jobDetail == null)
        //         {
        //             return null;
        //         }
        //         var result = _mapper.JobDetailToJobDetailResponseDto(jobDetail);
        //         return result;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception(ex.Message);
        //     }
        // }
    }
}
