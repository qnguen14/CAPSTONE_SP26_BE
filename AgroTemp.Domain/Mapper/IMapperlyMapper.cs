using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Farm;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.DTO.WorkerAttendance;
using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.Mapper;

public interface IMapperlyMapper
{
    // User
    UserDTO UserToUserDto(User user);
    List<UserDTO> UsersToUserDtos(IEnumerable<User> users);
    
    // FarmerProfile
    FarmerProfileDTO FarmerProfileToDto(FarmerProfile farmerProfile);

    //Farm
    FarmDTO FarmToDto(Farm farm);
    List<FarmDTO> FarmsToDto(IEnumerable<Farm> farms);

    // WorkerProfile
    WorkerProfileDTO WorkerProfileToDto(WorkerProfile workerProfile);
    
    // LoginResponse UserToLoginResponse(User user);
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
    JobPost CreateJobPostRequestToJobPost(CreateJobPostRequest request);
    void UpdateJobPostRequestToJobPost(UpdateJobPostRequest request, JobPost jobPost);

    // WorkerAttendance
    WorkerAttendanceDTO WorkerAttendanceToDto(Entities.WorkerAttendance workerAttendance);
    List<WorkerAttendanceDTO> WorkerAttendancesToDtos(IEnumerable<Entities.WorkerAttendance> workerAttendances);
}
