using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;
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
        Task<List<JobPostDTO>> GetJobPostsByFarmerId();
        Task<List<JobPostDTO>> GetFarmerJobHistory();
        Task<List<JobPostDTO>> GetJobPostsByStatus(JobPostStatus status);
        Task<List<WorkersPerDayDTO>> GetAcceptedWorkersPerDayAsync(Guid jobPostId);


        Task<JobPostDTO> CreateJobPost(CreateJobPostRequest request);
        Task<JobPostDTO> UpdateJobPost(Guid id, UpdateJobPostRequest request);
        Task<bool> DeleteJobPost(string id);


        Task<JobPostDTO> CancelJobPost(Guid id);


        Task<JobPostDTO> UpdateJobPostUrgency(string id, bool isUrgent);
        Task<JobPostDTO> UpdateJobPostStatus(string id, JobPostStatus status);


        Task<List<JobPostDTO>> GetFilteredJobPosts(string? title, string? category, string? address, List<string?> skill, bool sortByDateDesc = true);
        Task<List<JobPostDTO>> GetFilteredJobPostsByFarmer(string? title, string? category, string? address, List<string?> skill, bool sortByDateDesc = true);


        Task<JobPostDTO> SaveJobPostDraft(CreateJobPostRequest request);
        Task<List<JobPostDTO>> GetFarmerDrafts();


        Task<PaginatedJobDiscoveryResponse> SearchJobsAsync(JobSearchFilterRequest filter);
        Task<List<JobDiscoveryDTO>> GetNearbyJobsAsync(decimal latitude, decimal longitude, double maxDistanceKm = 20);
        Task<List<JobDiscoveryDTO>> GetJobsByDateAsync(string dateFilter);
        Task<List<JobDiscoveryDTO>> GetJobsBySkillAsync(List<string> skills);
        Task<List<JobDiscoveryDTO>> GetJobsByWageRangeAsync(decimal minWage, decimal? maxWage = null);
        Task<List<JobDiscoveryDTO>> GetJobsByTypeAsync(int jobTypeId);
        Task<List<JobDiscoveryDTO>> GetUrgentJobsAsync(decimal latitude, decimal longitude, double maxDistanceKm = 20);


        Task<SavedJobPostDTO> ToggleSaveJobPostAsync(Guid jobPostId);
        Task<List<SavedJobPostDTO>> GetSavedJobPostsAsync();
    }
}
