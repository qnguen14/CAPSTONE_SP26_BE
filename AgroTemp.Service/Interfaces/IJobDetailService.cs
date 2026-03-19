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
        Task<JobDetailDTO> CreateJobDetail(CreateJobDetailRequest request);
        Task<JobDetailDTO> UpdateJobDetail(UpdateJobDetailRequest request);
        Task<bool> DeleteJobDetail(string id);
        Task<JobDetailDTO> UpdateJobDetailStatus(string id, string status);
    }
}
