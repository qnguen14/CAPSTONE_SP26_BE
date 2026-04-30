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

        public async Task<JobDetailResponseDTO> GetJobDetailById(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var jobDetail = await _unitOfWork.GetRepository<JobDetail>()
                    .FirstOrDefaultAsync(
                        predicate: jd => jd.Id == guid,
                        include: jd => jd.Include(x => x.Worker).ThenInclude(w => w.User)
                                         .Include(x => x.JobPost).ThenInclude(jp => jp.Farmer).ThenInclude(f => f.User)
                                         .Include(x => x.JobAttachments));
                if (jobDetail == null)
                {
                    return null;
                }
                var result = _mapper.JobDetailToJobDetailResponseDto(jobDetail);
                result.Attachments = jobDetail.JobAttachments
                    .Select(a => new JobAttachmentDTO
                    {
                        Id = a.Id,
                        JobDetailId = a.JobDetailId,
                        CloudinaryPublicId = a.CloudinaryPublicId,
                        FileUrl = a.FileUrl,
                        Format = a.Format,
                        FileSize = a.FileSize,
                        CreatedAt = a.CreatedAt
                    }).ToList();

                // Map Farmer from JobPost.Farmer if available
                if (jobDetail.JobPost != null && jobDetail.JobPost.Farmer != null)
                {
                    result.Farmer = _mapper.FarmerToDto(jobDetail.JobPost.Farmer);
                }

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
                    .FirstOrDefaultAsync(predicate: ja => ja.Id.ToString() == id.ToString(),
                                         include: q => q.Include(x => x.Worker).ThenInclude(w => w.User).Include(x => x.JobPost));

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

                // Save image URL attachments
                if (request.ImageUrls != null && request.ImageUrls.Count > 0)
                {
                    var attachments = request.ImageUrls.Select(url => new JobAttachment
                    {
                        Id = Guid.NewGuid(),
                        JobDetailId = jobDetail.Id,
                        CloudinaryPublicId = Path.GetFileNameWithoutExtension(new Uri(url).AbsolutePath),
                        FileUrl = url,
                        Format = Path.GetExtension(url).TrimStart('.').ToLower(),
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                    await _unitOfWork.GetRepository<JobAttachment>().InsertRangeAsync(attachments);
                }

                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.JobDetailToJobDetailResponseDto(jobDetail);
                result.Attachments = jobDetail.JobAttachments
                    .Select(a => new JobAttachmentDTO
                    {
                        Id = a.Id,
                        JobDetailId = a.JobDetailId,
                        CloudinaryPublicId = a.CloudinaryPublicId,
                        FileUrl = a.FileUrl,
                        Format = a.Format,
                        FileSize = a.FileSize,
                        CreatedAt = a.CreatedAt
                    }).ToList();
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
                    include: q => q.Include(x => x.Worker).ThenInclude(w => w.User)
                                    .Include(x => x.JobPost).ThenInclude(jp => jp.Farmer).ThenInclude(f => f.User)
                                    .Include(x => x.JobAttachments));

                var items = await query.Skip(skip).Take(limit).ToListAsync();
                var data = _mapper.JobDetailsToJobDetailResponseDtos(items);

                // Fix mapping for Farmer and Attachments
                for (int i = 0; i < items.Count; i++)
                {
                    var jobDetail = items[i];
                    var dto = data[i];
                    // Attachments
                    dto.Attachments = jobDetail.JobAttachments
                        .Select(a => new JobAttachmentDTO
                        {
                            Id = a.Id,
                            JobDetailId = a.JobDetailId,
                            CloudinaryPublicId = a.CloudinaryPublicId,
                            FileUrl = a.FileUrl,
                            Format = a.Format,
                            FileSize = a.FileSize,
                            CreatedAt = a.CreatedAt
                        }).ToList();
                    // Farmer
                    if (jobDetail.JobPost != null && jobDetail.JobPost.Farmer != null)
                    {
                        dto.Farmer = _mapper.FarmerToDto(jobDetail.JobPost.Farmer);
                    }
                }

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
                    include: q => q.Include(x => x.Worker).ThenInclude(w => w.User)
                                    .Include(x => x.JobPost).ThenInclude(jp => jp.Farmer).ThenInclude(f => f.User)
                                    .Include(x => x.JobAttachments));

                var items = await query.Skip(skip).Take(limit).ToListAsync();
                var data = _mapper.JobDetailsToJobDetailResponseDtos(items);

                // Fix mapping for Farmer and Attachments
                for (int i = 0; i < items.Count; i++)
                {
                    var jobDetail = items[i];
                    var dto = data[i];
                    // Attachments
                    dto.Attachments = jobDetail.JobAttachments
                        .Select(a => new JobAttachmentDTO
                        {
                            Id = a.Id,
                            JobDetailId = a.JobDetailId,
                            CloudinaryPublicId = a.CloudinaryPublicId,
                            FileUrl = a.FileUrl,
                            Format = a.Format,
                            FileSize = a.FileSize,
                            CreatedAt = a.CreatedAt
                        }).ToList();
                    // Farmer
                    if (jobDetail.JobPost != null && jobDetail.JobPost.Farmer != null)
                    {
                        dto.Farmer = _mapper.FarmerToDto(jobDetail.JobPost.Farmer);
                    }
                }

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
                    .FirstOrDefaultAsync(jd => jd.Id == id, null, null);
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

                if (worker != null)
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

                // Determine job type and whether this is the last job detail
                var jobPost = await _unitOfWork.GetRepository<JobPost>()
                    .FirstOrDefaultAsync(predicate: jp => jp.Id == jobDetail.JobPostId);
                if (jobPost == null)
                    throw new Exception("Job post not found");

                var jobType = (JobType)jobPost.JobTypeId;
                bool isLastDetail;

                if (jobType == JobType.Daily)
                {
                    // Daily: release escrow on every approved report
                    isLastDetail = true;
                }
                else
                {
                    // PerJob: release escrow only on the last work day
                    // "Last day" = WorkDate matches EndDate, or the last entry in JobPostDays
                    var workDate = jobDetail.WorkDate.HasValue
                        ? DateOnly.FromDateTime(jobDetail.WorkDate.Value)
                        : (DateOnly?)null;

                    if (jobPost.EndDate.HasValue && workDate.HasValue)
                    {
                        isLastDetail = workDate.Value >= jobPost.EndDate.Value;
                    }
                    else if (jobPost.JobPostDays != null && jobPost.JobPostDays.Count > 0 && workDate.HasValue)
                    {
                        var lastSelectedDay = jobPost.JobPostDays.Max(jpd => jpd.WorkDate);
                        isLastDetail = workDate.Value >= lastSelectedDay;
                    }
                    else
                    {
                        // Fallback: treat as last if no date range is defined
                        isLastDetail = true;
                    }
                }

                if (jobType == JobType.PerJob && !isLastDetail)
                {
                    throw new Exception("Per-plot jobs can only be approved on the last report day");
                }

                await _walletService.ReleaseEscrowAndPayWorkerAsync(jobDetail, workerPayment, refund, isLastDetail);

                _unitOfWork.GetRepository<JobDetail>().UpdateAsync(jobDetail);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.JobDetailToJobDetailResponseDto(jobDetail);
                // Attachments
                result.Attachments = jobDetail.JobAttachments
                    .Select(a => new JobAttachmentDTO
                    {
                        Id = a.Id,
                        JobDetailId = a.JobDetailId,
                        CloudinaryPublicId = a.CloudinaryPublicId,
                        FileUrl = a.FileUrl,
                        Format = a.Format,
                        FileSize = a.FileSize,
                        CreatedAt = a.CreatedAt
                    }).ToList();
                // Farmer
                if (jobDetail.JobPost != null && jobDetail.JobPost.Farmer != null)
                {
                    result.Farmer = _mapper.FarmerToDto(jobDetail.JobPost.Farmer);
                }
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
