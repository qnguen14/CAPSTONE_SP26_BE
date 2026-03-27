using AgroTemp.Domain.DTO.Job.JobDetail;
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
        Task<JobDetailDTO> GetJobDetailById(string id);
        Task<JobDetailResponseDTO> GetById(string id); // new method returns JobDetailResponseDTO
        Task<JobDetailDTO> CreateJobDetail(CreateJobDetailRequest request);
        Task<JobDetailDTO> UpdateJobDetail(Guid id, UpdateJobDetailRequest request);
        Task<bool> DeleteJobDetail(string id);
        Task<JobDetailDTO> UpdateJobDetailStatus(string id, string status);

        Task<JobDetailResponseDTO> ReportDailyWork(CreateDailyReportRequest request);
        Task<List<JobDetailResponseDTO>> GetJobDetailsByWorkerId(Guid workerId);
        Task<List<JobDetailResponseDTO>> GetJobDetailsByJobPostId(Guid jobPostId);
        Task<JobDetailResponseDTO> ApproveJobDetail(Guid id, ApproveJobDetailRequest request);
    }
}
