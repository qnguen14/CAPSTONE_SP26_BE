using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Auth;
using AgroTemp.Domain.DTO.DisputeReport;
using AgroTemp.Domain.DTO.Farm;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.DTO.Job.JobCategory;
using AgroTemp.Domain.DTO.Job.JobDetail;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.DTO.Rating;
using AgroTemp.Domain.DTO.Skill;
using AgroTemp.Domain.DTO.WorkerAttendance;
using AgroTemp.Domain.Entities;
using Riok.Mapperly.Abstractions;
using JobApplicationEntity = AgroTemp.Domain.Entities.JobApplication;

namespace AgroTemp.Domain.Mapper;

[Mapper]
public partial class MapperlyMapper : IMapperlyMapper
{
    [MapProperty(nameof(User.Role), nameof(UserDTO.Role))]
    public partial UserDTO UserToUserDto(User user);

    public partial List<UserDTO> UsersToUserDtos(IEnumerable<User> users);

    public FarmerProfileDTO FarmerToDto(Farmer farmer)
    {
        return new FarmerProfileDTO
        {
            Id = farmer.Id,
            UserId = farmer.UserId,
            ContactName = farmer.ContactName,
            Address = farmer.Address,
            DateOfBirth = farmer.DateOfBirth,
            AverageRating = farmer.AverageRating,
            TotalJobsPosted = farmer.TotalJobsPosted,
            TotalJobsCompleted = farmer.TotalJobsCompleted,
            CreatedAt = farmer.CreatedAt,
            UpdatedAt = farmer.UpdatedAt,
            AvatarUrl = farmer.AvatarUrl,
            User = farmer.User != null ? UserToUserDto(farmer.User) : null
        };
    }

    //Farm
    [MapProperty(nameof(Farm.FarmerId), nameof(FarmDTO.FarmerProfileId))]
    public partial FarmDTO FarmToDto(Farm farm);
    public partial List<FarmDTO> FarmsToDto(IEnumerable<Farm> farms);

    public WorkerProfileDTO WorkerToDto(Worker worker)
    {
        return new WorkerProfileDTO
        {
            Id = worker.Id,
            UserId = worker.UserId,
            FullName = worker.FullName,
            Date_of_birth = worker.DateOfBirth.ToString(),
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
    [MapProperty(nameof(User.IsVerified), nameof(LoginResponse.IsVerified))]
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
    [MapProperty(nameof(JobPost.Farmer.ContactName), nameof(JobPostDTO.ContactName))]
    public partial JobPostDTO JobPostToJobPostDto(JobPost jobPost);
    public partial List<JobPostDTO> JobPostsToJobPostDtos(IEnumerable<JobPost> jobPosts);

    // JobDiscovery
    public JobDiscoveryDTO JobPostToJobDiscoveryDto(JobPost jobPost)
    {
        if (jobPost == null) return null;

        var jobTypeName = jobPost.JobTypeId switch
        {
            1 => "Daily",
            2 => "PerPlot",
            3 => "PerJob",
            _ => "Unknown"
        };

        var startDate = jobPost.StartDate ?? (jobPost.SelectedDays?.FirstOrDefault());
        var endDate = jobPost.EndDate ?? (jobPost.SelectedDays?.LastOrDefault());
        int? durationDays = null;
        if (startDate.HasValue && endDate.HasValue)
        {
            durationDays = endDate.Value.DayNumber - startDate.Value.DayNumber + 1;
        }

        var isUpcoming = startDate.HasValue && startDate.Value.ToDateTime(TimeOnly.MinValue) > DateTime.UtcNow && 
                        startDate.Value.ToDateTime(TimeOnly.MinValue) <= DateTime.UtcNow.AddDays(7);

        return new JobDiscoveryDTO
        {
            Id = jobPost.Id,
            FarmerProfileId = jobPost.FarmerId,
            ContactName = jobPost.Farmer?.ContactName,
            FarmId = jobPost.FarmId,
            JobCategoryId = jobPost.JobCategoryId,
            Title = jobPost.Title,
            Description = jobPost.Description,
            Address = jobPost.Address,
            StartDate = jobPost.StartDate,
            EndDate = jobPost.EndDate,
            SelectedDays = jobPost.SelectedDays,
            StartTime = jobPost.StartTime,
            EndTime = jobPost.EndTime,
            WorkersNeeded = jobPost.WorkersNeeded,
            WorkersAccepted = jobPost.WorkersAccepted,
            JobTypeId = jobPost.JobTypeId,
            JobTypeName = jobTypeName,
            WageAmount = jobPost.WageAmount,
            Requirements = jobPost.Requirements,
            Privileges = jobPost.Privileges,
            PublishedAt = jobPost.PublishedAt,
            IsUrgent = jobPost.IsUrgent,
            StatusId = jobPost.StatusId,
            FarmerAverageRating = jobPost.Farmer?.AverageRating ?? 0,
            LocationName = jobPost.Farm?.LocationName,
            SkillsMatchCount = jobPost.JobSkillRequirements?.Count ?? 0,
            AllSkillsMatched = false,
            DurationDays = durationDays,
            IsUpcoming = isUpcoming,
            MatchScore = 50, // Default neutral score
            SimilarJobsCompleted = 0,
            JobSkillRequirements = jobPost.JobSkillRequirements?.Select(jsr => new JobSkillRequirementSummaryDTO
            {
                Id = jsr.SkillId,
                Name = jsr.Skill?.Name
            }).ToList() ?? new List<JobSkillRequirementSummaryDTO>()
        };
    }

    public List<JobDiscoveryDTO> JobPostsToJobDiscoveryDtos(IEnumerable<JobPost> jobPosts)
    {
        return jobPosts?.Select(JobPostToJobDiscoveryDto).ToList() ?? new List<JobDiscoveryDTO>();
    }

    // JobSkillRequirement
    [MapProperty(nameof(JobSkillRequirement.SkillId), nameof(JobSkillRequirementSummaryDTO.Id))]
    [MapProperty("Skill.Name", nameof(JobSkillRequirementSummaryDTO.Name))]
    public partial JobSkillRequirementSummaryDTO JobSkillRequirementToSummaryDto(JobSkillRequirement jobSkillRequirement);
    public partial List<JobSkillRequirementSummaryDTO> JobSkillRequirementsToSummaryDtos(IEnumerable<JobSkillRequirement> jobSkillRequirements);
    public partial JobPost CreateJobPostRequestToJobPost(CreateJobPostRequest request);
    public partial void UpdateJobPostRequestToJobPost(UpdateJobPostRequest request, JobPost jobPost);

    // JobApplication
    public JobApplicationDTO JobApplicationToJobApplicationDto(JobApplicationEntity jobApplication)
    {
        var dto = new JobApplicationDTO
        {
            Id = jobApplication.Id,
            JobPostId = jobApplication.JobPostId,
            JobPost = jobApplication.JobPost != null ? JobPostToJobPostDto(jobApplication.JobPost) : null,
            Worker = jobApplication.Worker != null ? WorkerToDto(jobApplication.Worker) : null,
            StatusId = jobApplication.StatusId,
            CoverLetter = jobApplication.CoverLetter,
            AppliedAt = jobApplication.AppliedAt,
            RespondedAt = jobApplication.RespondedAt,
            ResponseMessage = jobApplication.ResponseMessage,
            WorkDates = jobApplication.WorkDates,
            LocationName = jobApplication.JobPost?.Farm?.LocationName
        };
        return dto;
    }
    public partial List<JobApplicationDTO> JobApplicationsToJobApplicationDtos(IEnumerable<JobApplicationEntity> jobApplications);
    public partial JobApplicationEntity CreateJobApplicationRequestToJobApplication(CreateJobApplicationRequest request);
    public partial void UpdateJobApplicationRequestToJobApplication(UpdateJobApplicationRequest request, JobApplicationEntity jobApplication);

    // Notification
    public partial NotificationDTO NotificationToDto(Notification notification);
    public partial List<NotificationDTO> NotificationsToDto(IEnumerable<Notification> notifications);

    // JobDetail
    public partial JobDetailDTO JobDetailToJobDetailDto(JobDetail jobDetail);
    public partial List<JobDetailDTO> JobDetailsToJobDetailDtos(IEnumerable<JobDetail> jobDetails);
    public partial JobDetailResponseDTO JobDetailToJobDetailResponseDto(JobDetail jobDetail);
    public partial List<JobDetailResponseDTO> JobDetailsToJobDetailResponseDtos(IEnumerable<JobDetail> jobDetails);
    public partial JobDetail CreateJobDetailRequestToJobDetail(CreateJobDetailRequest request);
    public partial void UpdateJobDetailRequestToJobDetail(UpdateJobDetailRequest request, JobDetail jobDetail);

    // Skill
    [MapProperty(nameof(Skill.JobCategoryId), nameof(SkillResponse.CategoryId))]
    public partial SkillResponse SkillToSkillResponse(Skill skill);
    public partial List<SkillResponse> SkillsToSkillResponses(IEnumerable<Skill> skills);
    [MapProperty(nameof(CreateSkillRequest.CategoryId), nameof(Skill.JobCategoryId))]
    public partial Skill CreateSkillRequestToSkill(CreateSkillRequest request);
    [MapProperty(nameof(UpdateSkillRequest.CategoryId), nameof(Skill.JobCategoryId))]
    public partial void UpdateSkillRequestToSkill(UpdateSkillRequest request, Skill skill);

    // Rating
    public partial RatingDTO RatingToRatingDto(Rating rating);
    public partial List<RatingDTO> RatingsToRatingDtos(IEnumerable<Rating> ratings);
    public partial Rating CreateRatingRequestToRating(CreateRatingRequest request);
    public partial void UpdateRatingRequestToRating(UpdateRatingRequest request, Rating rating);

    // Dispute
    public partial DisputeReportDTO DisputeReportToDisputeReportDto(DisputeReport disputeReport);
    public partial List<DisputeReportDTO> DisputeReportsToDisputeReportDtos(IEnumerable<DisputeReport> disputeReports);
    public partial DisputeReport CreateDisputeReportRequestToDisputeReport(CreateDisputeReportRequest request);
    public partial void UpdateDisputeReportRequestToDisputeReport(UpdateDisputeReportRequest request, DisputeReport disputeReport);

    // WorkerSession
    [MapProperty(nameof(WorkerSession.JobDetail.JobApplicationId), nameof(WorkerAttendanceDTO.JobApplicationId))]
    public partial WorkerAttendanceDTO WorkerSessionToDto(WorkerSession workerSession);
    public partial List<WorkerAttendanceDTO> WorkerSessionsToDtos(IEnumerable<WorkerSession> workerSessions);
}
