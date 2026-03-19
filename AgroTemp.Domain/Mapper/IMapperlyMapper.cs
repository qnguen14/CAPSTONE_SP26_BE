using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Auth;
using AgroTemp.Domain.DTO.Farm;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.DTO.Job.JobDetail;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.DTO.Skill;
using AgroTemp.Domain.DTO.WorkerAttendance;
using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.Mapper;

public interface IMapperlyMapper
{
    // User
    UserDTO UserToUserDto(User user);
    List<UserDTO> UsersToUserDtos(IEnumerable<User> users);
    
    // Farmer
    FarmerProfileDTO FarmerToDto(Farmer farmer);

    //Farm
    FarmDTO FarmToDto(Farm farm);
    List<FarmDTO> FarmsToDto(IEnumerable<Farm> farms);

    // Worker
    WorkerProfileDTO WorkerToDto(Worker worker);
    
    LoginResponse UserToLoginResponse(User user);
    // User RegisterRequestToUser(RegisterRequest resquest);
    // User RequestDTOToUser(UserRequestDTO request);
    // User UpdateProfileToUser(UpdateProfileRequest request);

    // JobCategory
    JobCategoryDTO JobCategoryToJobCategoryDto(JobCategory jobCategory);
    List<JobCategoryDTO> JobCategoriesToJobCategoryDtos(IEnumerable<JobCategory> jobCategories);
    JobCategory CreateJobCategoryRequestToJobCategory(CreateJobCategoryRequest request);
    void UpdateJobCategoryRequestToJobCategory(UpdateJobCategoryRequest request, JobCategory jobCategory);

    // JobPost
    JobPostDTO JobPostToJobPostDto(JobPost jobPost);
    List<JobPostDTO> JobPostsToJobPostDtos(IEnumerable<JobPost> jobPosts);
    JobSkillRequirementSummaryDTO JobSkillRequirementToSummaryDto(JobSkillRequirement jobSkillRequirement);
    List<JobSkillRequirementSummaryDTO> JobSkillRequirementsToSummaryDtos(IEnumerable<JobSkillRequirement> jobSkillRequirements);
    JobPost CreateJobPostRequestToJobPost(CreateJobPostRequest request);
    void UpdateJobPostRequestToJobPost(UpdateJobPostRequest request, JobPost jobPost);

    // JobApplication
    JobApplicationDTO JobApplicationToJobApplicationDto(JobApplication jobApplication);
    List<JobApplicationDTO> JobApplicationsToJobApplicationDtos(IEnumerable<JobApplication> jobApplications);
    JobApplication CreateJobApplicationRequestToJobApplication(CreateJobApplicationRequest request);
    void UpdateJobApplicationRequestToJobApplication(UpdateJobApplicationRequest request, JobApplication jobApplication);
    
    // JobDetail
    JobDetailDTO JobDetailToJobDetailDto(JobDetail jobDetail);
    List<JobDetailDTO> JobDetailsToJobDetailDtos(IEnumerable<JobDetail> jobDetails);
    JobDetail CreateJobDetailRequestToJobDetail(CreateJobDetailRequest request);
    void UpdateJobDetailRequestToJobDetail(UpdateJobDetailRequest request, JobDetail jobDetail);

    // Notification
    NotificationDTO NotificationToDto(Notification notification);
    List<NotificationDTO> NotificationsToDto(IEnumerable<Notification> notifications);

    // Skill
    SkillResponse SkillToSkillResponse(Skill skill);
    List<SkillResponse> SkillsToSkillResponses(IEnumerable<Skill> skills);
    Skill CreateSkillRequestToSkill(CreateSkillRequest request);
    void UpdateSkillRequestToSkill(UpdateSkillRequest request, Skill skill);

    // WorkerSession
    WorkerAttendanceDTO WorkerSessionToDto(WorkerSession workerSession);
    List<WorkerAttendanceDTO> WorkerSessionsToDtos(IEnumerable<WorkerSession> workerSessions);
}
