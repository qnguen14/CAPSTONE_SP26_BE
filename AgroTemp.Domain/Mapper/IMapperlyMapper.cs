using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.Mapper;

public interface IMapperlyMapper
{
    // User
    UserDTO UserToUserDto(User user);
    List<UserDTO> UsersToUserDtos(IEnumerable<User> users);
    
    // FarmerProfile
    FarmerProfileDTO FarmerProfileToDto(FarmerProfile farmerProfile);
    
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

    // JobPost
    JobPostDTO JobPostToJobPostDto(JobPost jobPost);
    List<JobPostDTO> JobPostsToJobPostDtos(IEnumerable<JobPost> jobPosts);
    JobPost CreateJobPostRequestToJobPost(CreateJobPostRequest request);
}