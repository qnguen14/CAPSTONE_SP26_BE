using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace AgroTemp.Domain.Mapper;

[Mapper]
public partial class MapperlyMapper : IMapperlyMapper
{
    [MapProperty(nameof(User.Role), nameof(UserDTO.Role))]
    public partial UserDTO UserToUserDto(User user);
    
    public partial List<UserDTO> UsersToUserDtos(IEnumerable<User> users);
    
    public partial FarmerProfileDTO FarmerProfileToDto(FarmerProfile farmerProfile);
    
    public partial WorkerProfileDTO WorkerProfileToDto(WorkerProfile workerProfile);
    
    // Custom mapping for ExperienceLevel enum to string
    private string MapExperienceLevel(ExperienceLevel level) => level.ToString();

    // JobCategory
    public partial JobCategoryDTO JobCategoryToJobCategoryDto(JobCategory jobCategory);
    public partial List<JobCategoryDTO> JobCategoriesToJobCategoryDtos(IEnumerable<JobCategory> jobCategories);
    public partial JobCategory CreateJobCategoryRequestToJobCategory(CreateJobCategoryRequest request);
    public partial void UpdateJobCategoryRequestToJobCategory(UpdateJobCategoryRequest request, JobCategory jobCategory);

    // JobPost
    [MapProperty(nameof(JobPost.PaymentMethod), nameof(JobPostDTO.PaymentMethodId))]
    [MapProperty(nameof(JobPost.Status), nameof(JobPostDTO.StatusId))]
    [MapProperty(nameof(JobPost.WageType), nameof(JobPostDTO.WageTypeId))]
    public partial JobPostDTO JobPostToJobPostDto(JobPost jobPost);
    public partial List<JobPostDTO> JobPostsToJobPostDtos(IEnumerable<JobPost> jobPosts);
    public partial JobPost CreateJobPostRequestToJobPost(CreateJobPostRequest request);
    public partial void UpdateJobPostRequestToJobPost(UpdateJobPostRequest request, JobPost jobPost);
}