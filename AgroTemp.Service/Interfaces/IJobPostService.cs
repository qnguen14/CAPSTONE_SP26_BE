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
        Task<JobPostDTO> UpdateJobPost(Guid id, UpdateJobPostRequest request);
        Task<bool> DeleteJobPost(string id);
        Task<JobPostDTO> UpdateJobPostUrgency(string id, bool isUrgent);
        Task<JobPostDTO> UpdateJobPostStatus(string id, string status);
        Task<List<JobPostDTO>> GetFilteredJobPosts(string? title, string? category, string? address, string? skill);
        Task<JobPostDTO> SaveJobPostDraft(CreateJobPostRequest request);
        Task<List<JobPostDTO>> GetFarmerDrafts();
    }
}
