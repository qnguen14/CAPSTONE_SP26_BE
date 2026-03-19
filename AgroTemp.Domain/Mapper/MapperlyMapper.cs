using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Auth;
using AgroTemp.Domain.DTO.Farm;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.DTO.WorkerAttendance;
using AgroTemp.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace AgroTemp.Domain.Mapper;

[Mapper]
public partial class MapperlyMapper : IMapperlyMapper
{
    [MapProperty(nameof(User.Role), nameof(UserDTO.Role))]
    public partial UserDTO UserToUserDto(User user);
    
    public partial List<UserDTO> UsersToUserDtos(IEnumerable<User> users);
    
    [MapProperty("User.Email", nameof(FarmerProfileDTO.Email))]
    public partial FarmerProfileDTO FarmerToDto(Farmer farmer);

    //Farm
    public partial FarmDTO FarmToDto(Farm farm);
    public partial List<FarmDTO> FarmsToDto(IEnumerable<Farm> farms);

    public WorkerProfileDTO WorkerToDto(Worker worker)
    {
        return new WorkerProfileDTO
        {
            Id = worker.Id,
            UserId = worker.UserId,
            FullName = worker.FullName,
            AgeRange = worker.AgeRange,
            PrimaryLocation = worker.PrimaryLocation,
            TravelRadiusKmPreference = worker.TravelRadiusKmPreference,
            ExperienceLevelId = worker.ExperienceLevelId,
            ExperienceLevel = MapExperienceLevel((ExperienceLevel)worker.ExperienceLevelId),
            AverageRating = worker.AverageRating,
            AvailabilitySchedule = worker.AvailabilitySchedule,
            TotalJobsCompleted = worker.TotalJobsCompleted,
            AvatarUrl = worker.AvatarUrl,
            CreatedAt = worker.CreatedAt,
            UpdatedAt = worker.UpdatedAt,
            Email = worker.User?.Email ?? string.Empty,
            PhoneNumber = worker.User?.PhoneNumber ?? string.Empty,
        };
    }

    [MapProperty(nameof(User.Role), nameof(LoginResponse.Role))]
    public partial LoginResponse UserToLoginResponse(User user);
    
    // Custom mapping for ExperienceLevel enum to string
    private string MapExperienceLevel(ExperienceLevel level) => level.ToString();

    private string MapUserRole(UserRole role) => role.ToString();

    private string MapStatusId(int statusId)
    {
        return Enum.IsDefined(typeof(JobPostStatus), statusId)
            ? ((JobPostStatus)statusId).ToString()
            : "Unknown";
    }

    // JobCategory
    public partial JobCategoryDTO JobCategoryToJobCategoryDto(JobCategory jobCategory);
    public partial List<JobCategoryDTO> JobCategoriesToJobCategoryDtos(IEnumerable<JobCategory> jobCategories);
    public partial JobCategory CreateJobCategoryRequestToJobCategory(CreateJobCategoryRequest request);
    public partial void UpdateJobCategoryRequestToJobCategory(UpdateJobCategoryRequest request, JobCategory jobCategory);

    // JobPost
    [MapProperty(nameof(JobPost.FarmerId), nameof(JobPostDTO.FarmerProfileId))]
    [MapProperty(nameof(JobPost.StatusId), nameof(JobPostDTO.Status), Use = nameof(MapStatusId))]
    public partial JobPostDTO JobPostToJobPostDto(JobPost jobPost);
    public partial List<JobPostDTO> JobPostsToJobPostDtos(IEnumerable<JobPost> jobPosts);
    [MapProperty(nameof(JobSkillRequirement.SkillId), nameof(JobSkillRequirementSummaryDTO.Id))]
    [MapProperty("Skill.Name", nameof(JobSkillRequirementSummaryDTO.Name))]
    public partial JobSkillRequirementSummaryDTO JobSkillRequirementToSummaryDto(JobSkillRequirement jobSkillRequirement);
    public partial List<JobSkillRequirementSummaryDTO> JobSkillRequirementsToSummaryDtos(IEnumerable<JobSkillRequirement> jobSkillRequirements);
    public partial JobPost CreateJobPostRequestToJobPost(CreateJobPostRequest request);
    public partial void UpdateJobPostRequestToJobPost(UpdateJobPostRequest request, JobPost jobPost);

    // JobApplication
    public partial JobApplicationDTO JobApplicationToJobApplicationDto(JobApplication jobApplication);
    public partial List<JobApplicationDTO> JobApplicationsToJobApplicationDtos(IEnumerable<JobApplication> jobApplications);
    public partial JobApplication CreateJobApplicationRequestToJobApplication(CreateJobApplicationRequest request);
    public partial void UpdateJobApplicationRequestToJobApplication(UpdateJobApplicationRequest request, JobApplication jobApplication);
    
    // Notification
    public partial NotificationDTO NotificationToDto(Notification notification);
    public partial List<NotificationDTO> NotificationsToDto(IEnumerable<Notification> notifications);

    // WorkerSession
    public partial WorkerAttendanceDTO WorkerSessionToDto(WorkerSession workerSession);
    public partial List<WorkerAttendanceDTO> WorkerSessionsToDtos(IEnumerable<WorkerSession> workerSessions);
}
