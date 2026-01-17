using AgroTemp.Domain.DTO.Job.JobPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Service.Interfaces
{
    public interface IJobPostService
    {
        Task<List<JobPostDTO>> GetAllJobPosts();
        Task<JobPostDTO> GetJobPostById(string id);
        Task<JobPostDTO> CreateJobPost(CreateJobPostRequest request);
        Task<JobPostDTO> UpdateJobPost(UpdateJobPostRequest request);
        Task<bool> DeleteJobPost(string id);
    }
}
