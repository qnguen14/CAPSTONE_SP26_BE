using AgroTemp.Domain.DTO.Job.JobApplication;

namespace AgroTemp.Service.Interfaces
{
    public interface IJobApplicationService
    {
        Task<List<JobApplicationDTO>> GetAllJobApplications();
        Task<JobApplicationDTO> GetJobApplicationById(string id);
        Task<JobApplicationDTO> CreateJobApplication(CreateJobApplicationRequest request);
        Task<JobApplicationDTO> UpdateJobApplication(Guid id, UpdateJobApplicationRequest request);
        Task<bool> DeleteJobApplication(string id);
        Task<JobApplicationDTO> RespondJobApplication(string id, RespondJobApplicationRequest request);
    }
}
