using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.Metadata;

namespace AgroTemp.Service.Interfaces
{
    public interface IJobApplicationService
    {
        Task<List<JobApplicationDTO>> GetAllJobApplications();
        Task<List<JobApplicationDTO>> GetJobApplicationsByWorker();
        Task<JobApplicationDTO> GetJobApplicationById(string id);
        Task<JobApplicationDTO> CreateJobApplication(CreateJobApplicationRequest request);
        Task<JobApplicationDTO> UpdateJobApplication(Guid id, UpdateJobApplicationRequest request);
        Task<bool> DeleteJobApplication(string id);
        Task<JobApplicationDTO> RespondJobApplication(string id, RespondJobApplicationRequest request);
        Task<PaginatedResponse<JobApplicationDTO>> GetJobApplicationsByJobPostId(Guid jobPostId, Guid farmerProfileId, int? statusId, bool includeAll, int page, int limit);
        Task<List<JobApplicationDTO>> AutoAcceptUrgentJobApplicationsAsync(List<Guid> jobApplicationIds);
        Task<JobApplicationDTO> CancelJobApplication(Guid id);
    }
}
