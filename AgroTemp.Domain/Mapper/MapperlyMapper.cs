using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Job;
using AgroTemp.Domain.DTO.JobCategory;
using AgroTemp.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace AgroTemp.Domain.Mapper;

[Mapper]
public partial class MapperlyMapper : IMapperlyMapper
{
    [MapProperty(nameof(User.Role), nameof(UserDTO.Role))]
    // [MapProperty(nameof(User.StudentTestSessions), nameof(UserDTO.StudentTestSessions))]
    // [MapProperty(nameof(User.Id), nameof(UserDTO.Id))]
    public partial UserDTO UserToUserDto(User user);
    
    public partial List<UserDTO> UsersToUserDtos(IEnumerable<User> users);

    // JobCategory
    public partial JobCategoryDTO JobCategoryToJobCategoryDto(JobCategory jobCategory);
    public partial List<JobCategoryDTO> JobCategoriesToJobCategoryDtos(IEnumerable<JobCategory> jobCategories);

    // JobPost
    [MapProperty(nameof(JobPost.PaymentMethod), nameof(JobPostDTO.PaymentMethodId))]
    [MapProperty(nameof(JobPost.Status), nameof(JobPostDTO.StatusId))]
    [MapProperty(nameof(JobPost.WageType), nameof(JobPostDTO.WageTypeId))]
    public partial JobPostDTO JobPostToJobPostDto(JobPost jobPost);
    public partial List<JobPostDTO> JobPostsToJobPostDtos(IEnumerable<JobPost> jobPosts);
}