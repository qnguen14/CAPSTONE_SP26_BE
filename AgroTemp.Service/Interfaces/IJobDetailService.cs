using AgroTemp.Domain.DTO.Job.JobDetail;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Service.Interfaces
{
    public interface IJobDetailService
    {
        Task<List<JobDetailDTO>> GetAllJobDetails();
        Task<JobDetailResponseDTO> GetJobDetailById(string id);
        // Task<JobDetailResponseDTO> GetById(string id); // new method returns JobDetailResponseDTO
        Task<JobDetailDTO> CreateJobDetail(CreateJobDetailRequest request);
        Task<JobDetailDTO> UpdateJobDetail(Guid id, UpdateJobDetailRequest request);
        Task<bool> DeleteJobDetail(string id);
        Task<JobDetailDTO> UpdateJobDetailStatus(string id, string status);

        Task<JobDetailResponseDTO> ReportDailyWork(Guid id, CreateDailyReportRequest request);
        Task<PaginatedResponse<JobDetailResponseDTO>> GetJobDetailsByWorkerId(Guid workerId, int page = 1, int limit = 10);
        Task<PaginatedResponse<JobDetailResponseDTO>> GetJobDetailsByJobPostId(Guid jobPostId, JobStatus? jobStatus, bool orderByDescending = true, int page = 1, int limit = 10);
        Task<JobDetailResponseDTO> ApproveJobDetail(Guid id,ApproveJobDetailRequest request);
    }
}
