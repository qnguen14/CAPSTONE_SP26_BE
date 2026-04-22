namespace AgroTemp.Test;


using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Job.JobApplication;
using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Implements;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;


public class JobServiceTest
{
    private readonly Mock<IUnitOfWork<AgroTempDbContext>> _unitOfWorkMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IMapperlyMapper> _mapperMock;
    private readonly Mock<IWalletService> _walletServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IGenericRepository<JobPost>> _jobPostRepoMock;
    private readonly Mock<IGenericRepository<Farmer>> _farmerRepoMock;
    private readonly Mock<IGenericRepository<Skill>> _skillRepoMock;
    private readonly Mock<IGenericRepository<JobSkillRequirement>> _skillReqRepoMock;
    private readonly Mock<IGenericRepository<JobApplication>> _jobAppRepoMock;
    private readonly Mock<IGenericRepository<Worker>> _workerRepoMock;
    private readonly JobPostService _jobPostService;
    private readonly JobApplicationService _jobApplicationService;

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Guid _farmerId = Guid.NewGuid();

    public JobServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork<AgroTempDbContext>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _mapperMock = new Mock<IMapperlyMapper>();
        _walletServiceMock = new Mock<IWalletService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _jobPostRepoMock = new Mock<IGenericRepository<JobPost>>();
        _farmerRepoMock = new Mock<IGenericRepository<Farmer>>();
        _skillRepoMock = new Mock<IGenericRepository<Skill>>();
        _skillReqRepoMock = new Mock<IGenericRepository<JobSkillRequirement>>();
        _jobAppRepoMock = new Mock<IGenericRepository<JobApplication>>();
        _workerRepoMock = new Mock<IGenericRepository<Worker>>();

        // User context
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _currentUserId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Repositories
        _unitOfWorkMock.Setup(u => u.GetRepository<JobPost>()).Returns(_jobPostRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<Farmer>()).Returns(_farmerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<Skill>()).Returns(_skillRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<JobSkillRequirement>()).Returns(_skillReqRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<JobApplication>()).Returns(_jobAppRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<Worker>()).Returns(_workerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _jobPostService = new JobPostService(
            _unitOfWorkMock.Object,
            _httpContextAccessorMock.Object,
            _mapperMock.Object,
            _walletServiceMock.Object,
            _notificationServiceMock.Object);

        _jobApplicationService = new JobApplicationService(
            _unitOfWorkMock.Object,
            _httpContextAccessorMock.Object,
            _mapperMock.Object,
            _notificationServiceMock.Object);
    }

    // Helpers

    private Farmer MakeFarmer() => new Farmer
    {
        Id = _farmerId,
        UserId = _currentUserId,
        ContactName = "Test Farmer",
        Address = "123 Farm Rd"
    };

    private JobPost MakeJobPost(Guid? id = null) => new JobPost
    {
        Id = id ?? Guid.NewGuid(),
        FarmerId = _farmerId,
        Title = "Harvest Help",
        Description = "Need workers",
        Address = "Farm",
        WageAmount = 100,
        WorkersNeeded = 2,
        StatusId = (int)JobPostStatus.Published,
        JobTypeId = (int)JobType.PerJob,
        JobSkillRequirements = new List<JobSkillRequirement>(),
        Farmer = MakeFarmer()
    };

    private JobPostDTO MakeJobPostDTO(Guid? id = null) => new JobPostDTO
    {
        Id = id ?? Guid.NewGuid(),
        Title = "Harvest Help",
        StatusId = (int)JobPostStatus.Published
    };

    // GetAllJobPosts

    [Fact]
    public async Task GetAllJobPosts_WhenJobPostsExist_ReturnsJobPostDTOList()
    {
        var jobPosts = new List<JobPost> { MakeJobPost(), MakeJobPost() };
        var dtos = jobPosts.Select(jp => MakeJobPostDTO(jp.Id)).ToList();

        _jobPostRepoMock
            .Setup(r => r.GetListAsync(null, It.IsAny<Func<IQueryable<JobPost>, IOrderedQueryable<JobPost>>>(), It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>(), null))
            .ReturnsAsync(jobPosts);
        _mapperMock.Setup(m => m.JobPostsToJobPostDtos(jobPosts)).Returns(dtos);

        var result = await _jobPostService.GetAllJobPosts();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllJobPosts_WhenNoJobPosts_ReturnsNull()
    {
        _jobPostRepoMock
            .Setup(r => r.GetListAsync(null, It.IsAny<Func<IQueryable<JobPost>, IOrderedQueryable<JobPost>>>(), It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>(), null))
            .ReturnsAsync(new List<JobPost>());

        var result = await _jobPostService.GetAllJobPosts();

        Assert.Null(result);
    }

    // GetJobPostById

    [Fact]
    public async Task GetJobPostById_WhenFound_ReturnsJobPostDTO()
    {
        var jobPost = MakeJobPost();
        var dto = MakeJobPostDTO(jobPost.Id);

        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(jobPost);
        _mapperMock.Setup(m => m.JobPostToJobPostDto(jobPost)).Returns(dto);

        var result = await _jobPostService.GetJobPostById(jobPost.Id.ToString());

        Assert.NotNull(result);
        Assert.Equal(jobPost.Id, result.Id);
    }

    [Fact]
    public async Task GetJobPostById_WhenNotFound_ReturnsNull()
    {
        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync((JobPost)null!);

        var result = await _jobPostService.GetJobPostById(Guid.NewGuid().ToString());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetJobPostById_WhenInvalidGuid_ThrowsException()
    {
        await Assert.ThrowsAsync<Exception>(() => _jobPostService.GetJobPostById("not-a-guid"));
    }

    // GetJobPostsByFarmerId

    [Fact]
    public async Task GetJobPostsByFarmerId_WhenFarmerExists_ReturnsJobPosts()
    {
        var farmer = MakeFarmer();
        var jobPosts = new List<JobPost> { MakeJobPost() };
        var dtos = new List<JobPostDTO> { MakeJobPostDTO() };

        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync(farmer);
        _jobPostRepoMock
            .Setup(r => r.GetListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), It.IsAny<Func<IQueryable<JobPost>, IOrderedQueryable<JobPost>>>(), It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>(), null))
            .ReturnsAsync(jobPosts);
        _mapperMock.Setup(m => m.JobPostsToJobPostDtos(jobPosts)).Returns(dtos);

        var result = await _jobPostService.GetJobPostsByFarmerId();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetJobPostsByFarmerId_WhenFarmerNotFound_ThrowsException()
    {
        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync((Farmer)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.GetJobPostsByFarmerId());
        Assert.Contains("not authorized", ex.Message);
    }

    [Fact]
    public async Task GetJobPostsByFarmerId_WhenNoJobPosts_ReturnsEmptyList()
    {
        var farmer = MakeFarmer();

        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync(farmer);
        _jobPostRepoMock
            .Setup(r => r.GetListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), It.IsAny<Func<IQueryable<JobPost>, IOrderedQueryable<JobPost>>>(), It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>(), null))
            .ReturnsAsync(new List<JobPost>());

        var result = await _jobPostService.GetJobPostsByFarmerId();

        Assert.Empty(result);
    }

    // CreateJobPost

    [Fact]
    public async Task CreateJobPost_WhenValid_ReturnsCreatedJobPostDTO()
    {
        var farmer = MakeFarmer();
        var jobPost = MakeJobPost();
        var dto = MakeJobPostDTO(jobPost.Id);
        var request = new CreateJobPostRequest
        {
            Title = "Harvest Help",
            Description = "Need workers",
            Address = "Farm",
            WageAmount = 100,
            WorkersNeeded = 2,
            JobTypeId = JobType.PerJob,
            SkillIds = new List<Guid>(),
            StatusId = (int)JobPostStatus.Published
        };

        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync(farmer);
        _mapperMock.Setup(m => m.CreateJobPostRequestToJobPost(request)).Returns(jobPost);
        _walletServiceMock
            .Setup(w => w.LockAmountForJobPostAsync(farmer.UserId, jobPost.Id, It.IsAny<decimal>()))
            .Returns(Task.CompletedTask);
        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(jobPost);
        _mapperMock.Setup(m => m.JobPostToJobPostDto(jobPost)).Returns(dto);

        var result = await _jobPostService.CreateJobPost(request);

        Assert.NotNull(result);
        _jobPostRepoMock.Verify(r => r.InsertAsync(jobPost), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateJobPost_WhenUserNotAuthenticated_ThrowsException()
    {
        // Override HttpContext to have empty user
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.CreateJobPost(new CreateJobPostRequest()));
        Assert.Contains("not authenticated", ex.Message);
    }

    [Fact]
    public async Task CreateJobPost_WhenFarmerNotFound_ThrowsException()
    {
        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync((Farmer)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.CreateJobPost(new CreateJobPostRequest()));
        Assert.Contains("farmers", ex.Message);
    }

    [Fact]
    public async Task CreateJobPost_WhenInvalidSkillIds_ThrowsException()
    {
        var farmer = MakeFarmer();
        var validSkillId = Guid.NewGuid();
        var invalidSkillId = Guid.NewGuid();

        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync(farmer);

        _skillRepoMock
            .Setup(r => r.GetListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Skill, bool>>>(), null, null, null))
            .ReturnsAsync(new List<Skill> { new Skill { Id = validSkillId } });

        var request = new CreateJobPostRequest
        {
            SkillIds = new List<Guid> { validSkillId, invalidSkillId },
            WageAmount = 100,
            WorkersNeeded = 1,
            JobTypeId = JobType.PerJob
        };

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.CreateJobPost(request));
        Assert.Contains("Invalid skill", ex.Message);
    }

    [Fact]
    public async Task CreateJobPost_WhenInsufficientWalletBalance_ThrowsException()
    {
        var farmer = MakeFarmer();
        var jobPost = MakeJobPost();
        var request = new CreateJobPostRequest
        {
            WageAmount = 500,
            WorkersNeeded = 1,
            JobTypeId = JobType.PerJob,
            SkillIds = new List<Guid>()
        };

        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync(farmer);
        _mapperMock.Setup(m => m.CreateJobPostRequestToJobPost(request)).Returns(jobPost);
        _walletServiceMock
            .Setup(w => w.LockAmountForJobPostAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()))
            .ThrowsAsync(new InvalidOperationException("Insufficient balance"));

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.CreateJobPost(request));
        Assert.Contains("Insufficient wallet balance", ex.Message);
    }

    // UpdateJobPost

    [Fact]
    public async Task UpdateJobPost_WhenFound_ReturnsUpdatedJobPostDTO()
    {
        var jobPost = MakeJobPost();
        var dto = MakeJobPostDTO(jobPost.Id);
        var request = new UpdateJobPostRequest { Title = "Updated Title", SkillIds = new List<Guid>() };

        _jobPostRepoMock
            .SetupSequence(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(jobPost)
            .ReturnsAsync(jobPost);
        _mapperMock.Setup(m => m.JobPostToJobPostDto(jobPost)).Returns(dto);

        var result = await _jobPostService.UpdateJobPost(jobPost.Id, request);

        Assert.NotNull(result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateJobPost_WhenNotFound_ReturnsNull()
    {
        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync((JobPost)null!);

        var result = await _jobPostService.UpdateJobPost(Guid.NewGuid(), new UpdateJobPostRequest());

        Assert.Null(result);
    }

    // DeleteJobPost

    [Fact]
    public async Task DeleteJobPost_WhenFound_ReturnsTrueAndDeletes()
    {
        var jobPost = MakeJobPost();

        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, null))
            .ReturnsAsync(jobPost);

        var result = await _jobPostService.DeleteJobPost(jobPost.Id.ToString());

        Assert.True(result);
        _jobPostRepoMock.Verify(r => r.DeleteAsync(jobPost), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteJobPost_WhenNotFound_ReturnsFalse()
    {
        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, null))
            .ReturnsAsync((JobPost)null!);

        var result = await _jobPostService.DeleteJobPost(Guid.NewGuid().ToString());

        Assert.False(result);
        _jobPostRepoMock.Verify(r => r.DeleteAsync(It.IsAny<JobPost>()), Times.Never);
    }

    [Fact]
    public async Task DeleteJobPost_WhenInvalidGuid_ThrowsException()
    {
        await Assert.ThrowsAsync<Exception>(() => _jobPostService.DeleteJobPost("invalid-guid"));
    }

    // CancelJobPost

    [Fact]
    public async Task CancelJobPost_WhenValid_CancelsAndRefunds()
    {
        var jobPost = MakeJobPost();
        jobPost.StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var dto = MakeJobPostDTO(jobPost.Id);
        dto.StatusId = (int)JobPostStatus.Cancelled;

        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(jobPost);
        _jobAppRepoMock
            .Setup(r => r.GetListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, bool>>>(), null, It.IsAny<Func<IQueryable<JobApplication>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobApplication, object>>>(), null))
            .ReturnsAsync(new List<JobApplication>());
        _walletServiceMock
            .Setup(w => w.RefundLockedAmountForJobPostAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.JobPostToJobPostDto(jobPost)).Returns(dto);

        var result = await _jobPostService.CancelJobPost(jobPost.Id);

        Assert.NotNull(result);
        Assert.Equal((int)JobPostStatus.Cancelled, result.StatusId);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelJobPost_WhenJobPostNotFound_ThrowsException()
    {
        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync((JobPost)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.CancelJobPost(Guid.NewGuid()));
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public async Task CancelJobPost_WhenNotOwner_ThrowsException()
    {
        var jobPost = MakeJobPost();
        jobPost.Farmer.UserId = Guid.NewGuid(); // Different user

        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(jobPost);

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.CancelJobPost(jobPost.Id));
        Assert.Contains("authorized", ex.Message);
    }

    [Fact]
    public async Task CancelJobPost_WhenAlreadyCancelled_ThrowsException()
    {
        var jobPost = MakeJobPost();
        jobPost.StatusId = (int)JobPostStatus.Cancelled;

        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(jobPost);

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.CancelJobPost(jobPost.Id));
        Assert.Contains("cannot be cancelled", ex.Message);
    }

    [Fact]
    public async Task CancelJobPost_WhenJobAlreadyStarted_ThrowsException()
    {
        var jobPost = MakeJobPost();
        jobPost.StatusId = (int)JobPostStatus.Published;
        jobPost.StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)); // Already started

        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(jobPost);

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.CancelJobPost(jobPost.Id));
        Assert.Contains("already started", ex.Message);
    }

    // GetJobPostsByStatus

    [Fact]
    public async Task GetJobPostsByStatus_WhenFarmerExists_ReturnsFilteredJobPosts()
    {
        var farmer = MakeFarmer();
        var jobPosts = new List<JobPost> { MakeJobPost() };
        var dtos = new List<JobPostDTO> { MakeJobPostDTO() };

        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync(farmer);
        _jobPostRepoMock
            .Setup(r => r.GetListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), It.IsAny<Func<IQueryable<JobPost>, IOrderedQueryable<JobPost>>>(), It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>(), null))
            .ReturnsAsync(jobPosts);
        _mapperMock.Setup(m => m.JobPostsToJobPostDtos(jobPosts)).Returns(dtos);

        var result = await _jobPostService.GetJobPostsByStatus(JobPostStatus.Published);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetJobPostsByStatus_WhenFarmerNotFound_ThrowsException()
    {
        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync((Farmer)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.GetJobPostsByStatus(JobPostStatus.Published));
        Assert.Contains("not authorized", ex.Message);
    }

    // TC_BE_003: Create job with start date in the past

    [Fact]
    public async Task TC_BE_003_CreateJobPost_WithPastStartDate_ThrowsValidationError()
    {
        // Arrange
        var farmer = MakeFarmer();
        var request = new CreateJobPostRequest
        {
            Title = "Old Job",
            Description = "Past job",
            Address = "Farm",
            WageAmount = 100,
            WorkersNeeded = 1,
            JobTypeId = JobType.PerJob,
            SkillIds = new List<Guid>(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)), // 3 days in the past
            StatusId = (int)JobPostStatus.Published
        };

        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync(farmer);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _jobPostService.CreateJobPost(request));
        Assert.Contains("start date cannot be in the past", ex.Message);
    }

    [Fact]
    public async Task TC_BE_003_CreateJobPost_WithFutureStartDate_Succeeds()
    {
        var farmer = MakeFarmer();
        var jobPost = MakeJobPost();
        var dto = MakeJobPostDTO(jobPost.Id);
        var request = new CreateJobPostRequest
        {
            Title = "Future Job",
            Description = "Valid job",
            Address = "Farm",
            WageAmount = 100,
            WorkersNeeded = 1,
            JobTypeId = JobType.PerJob,
            SkillIds = new List<Guid>(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), // 1 week from now
            StatusId = (int)JobPostStatus.Published
        };

        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync(farmer);
        _mapperMock.Setup(m => m.CreateJobPostRequestToJobPost(request)).Returns(jobPost);
        _walletServiceMock
            .Setup(w => w.LockAmountForJobPostAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()))
            .Returns(Task.CompletedTask);
        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(jobPost);
        _mapperMock.Setup(m => m.JobPostToJobPostDto(jobPost)).Returns(dto);

        // Act
        var result = await _jobPostService.CreateJobPost(request);

        // Assert
        Assert.NotNull(result);
    }

    // TC_BE_004: Geolocation query — only jobs within radius are returned

    [Fact]
    public async Task TC_BE_004_GetNearbyJobs_ReturnsOnlyJobsWithinRadius()
    {
        // Reference point: lat=10.0, lon=106.0 (Ho Chi Minh City), radius=10km
        decimal userLat = 10.0m;
        decimal userLon = 106.0m;
        double radiusKm = 10.0;

        var farmInside1 = new Farm { Id = Guid.NewGuid(), FarmerId = _farmerId, Address = "Farm A",
            Latitude = 10.05m, Longitude = 106.0m, LocationName = "Near Farm A" }; // ~5.6 km

        var farmInside2 = new Farm { Id = Guid.NewGuid(), FarmerId = _farmerId, Address = "Farm B",
            Latitude = 10.0m, Longitude = 106.07m, LocationName = "Near Farm B" }; // ~7.7 km

        var farmOutside = new Farm { Id = Guid.NewGuid(), FarmerId = _farmerId, Address = "Farm C",
            Latitude = 10.2m, Longitude = 106.0m, LocationName = "Far Farm C" }; // ~22 km

        var jobInside1Id = Guid.NewGuid();
        var jobInside2Id = Guid.NewGuid();
        var jobOutsideId = Guid.NewGuid();

        var publishedJobs = new List<JobPost>
        {
            new JobPost { Id = jobInside1Id, FarmerId = _farmerId, Title = "Job A",
                StatusId = (int)JobPostStatus.Published, Farm = farmInside1,
                JobSkillRequirements = new List<JobSkillRequirement>(), Farmer = MakeFarmer() },
            new JobPost { Id = jobInside2Id, FarmerId = _farmerId, Title = "Job B",
                StatusId = (int)JobPostStatus.Published, Farm = farmInside2,
                JobSkillRequirements = new List<JobSkillRequirement>(), Farmer = MakeFarmer() },
            new JobPost { Id = jobOutsideId, FarmerId = _farmerId, Title = "Job C",
                StatusId = (int)JobPostStatus.Published, Farm = farmOutside,
                JobSkillRequirements = new List<JobSkillRequirement>(), Farmer = MakeFarmer() }
        };

        _jobPostRepoMock
            .Setup(r => r.GetListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(),
                It.IsAny<Func<IQueryable<JobPost>, IOrderedQueryable<JobPost>>>(),
                It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>(),
                null))
            .ReturnsAsync(publishedJobs);

        _mapperMock.Setup(m => m.JobPostToJobDiscoveryDto(It.Is<JobPost>(jp => jp.Id == jobInside1Id)))
            .Returns(new JobDiscoveryDTO { Id = jobInside1Id, Title = "Job A" });
        _mapperMock.Setup(m => m.JobPostToJobDiscoveryDto(It.Is<JobPost>(jp => jp.Id == jobInside2Id)))
            .Returns(new JobDiscoveryDTO { Id = jobInside2Id, Title = "Job B" });
        _mapperMock.Setup(m => m.JobPostToJobDiscoveryDto(It.Is<JobPost>(jp => jp.Id == jobOutsideId)))
            .Returns(new JobDiscoveryDTO { Id = jobOutsideId, Title = "Job C" });

        var result = await _jobPostService.GetNearbyJobsAsync(userLat, userLon, radiusKm);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, j => j.Id == jobOutsideId);
        Assert.Contains(result, j => j.Id == jobInside1Id);
        Assert.Contains(result, j => j.Id == jobInside2Id);
    }

    [Fact]
    public async Task TC_BE_004_GetNearbyJobs_ResultsSortedByDistanceAscending()
    {
        // Arrange — three jobs all inside 10km, sorted nearest first
        decimal userLat = 10.0m;
        decimal userLon = 106.0m;

        var farmClose = new Farm { Id = Guid.NewGuid(), FarmerId = _farmerId, Address = "A",
            Latitude = 10.02m, Longitude = 106.0m, LocationName = "Close" };  // ~2.2 km
        var farmMid = new Farm { Id = Guid.NewGuid(), FarmerId = _farmerId, Address = "B",
            Latitude = 10.05m, Longitude = 106.0m, LocationName = "Mid" };    // ~5.6 km
        var farmFar = new Farm { Id = Guid.NewGuid(), FarmerId = _farmerId, Address = "C",
            Latitude = 10.08m, Longitude = 106.0m, LocationName = "Far" };    // ~8.9 km

        var closeId = Guid.NewGuid();
        var midId   = Guid.NewGuid();
        var farId   = Guid.NewGuid();

        var publishedJobs = new List<JobPost>
        {
            new JobPost { Id = farId,   FarmerId = _farmerId, Title = "Far",   StatusId = (int)JobPostStatus.Published, Farm = farmFar,   JobSkillRequirements = new List<JobSkillRequirement>(), Farmer = MakeFarmer() },
            new JobPost { Id = closeId, FarmerId = _farmerId, Title = "Close", StatusId = (int)JobPostStatus.Published, Farm = farmClose, JobSkillRequirements = new List<JobSkillRequirement>(), Farmer = MakeFarmer() },
            new JobPost { Id = midId,   FarmerId = _farmerId, Title = "Mid",   StatusId = (int)JobPostStatus.Published, Farm = farmMid,   JobSkillRequirements = new List<JobSkillRequirement>(), Farmer = MakeFarmer() }
        };

        _jobPostRepoMock
            .Setup(r => r.GetListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(),
                It.IsAny<Func<IQueryable<JobPost>, IOrderedQueryable<JobPost>>>(),
                It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>(),
                null))
            .ReturnsAsync(publishedJobs);

        _mapperMock.Setup(m => m.JobPostToJobDiscoveryDto(It.Is<JobPost>(jp => jp.Id == closeId))).Returns(new JobDiscoveryDTO { Id = closeId });
        _mapperMock.Setup(m => m.JobPostToJobDiscoveryDto(It.Is<JobPost>(jp => jp.Id == midId))).Returns(new JobDiscoveryDTO { Id = midId });
        _mapperMock.Setup(m => m.JobPostToJobDiscoveryDto(It.Is<JobPost>(jp => jp.Id == farId))).Returns(new JobDiscoveryDTO { Id = farId });

        var result = await _jobPostService.GetNearbyJobsAsync(userLat, userLon, 10.0);

        Assert.Equal(3, result.Count);
        Assert.True(result[0].DistanceKm <= result[1].DistanceKm);
        Assert.True(result[1].DistanceKm <= result[2].DistanceKm);
        Assert.Equal(closeId, result[0].Id);
    }

    // TC_BE_005: Apply to a full-capacity job is rejected

    [Fact]
    public async Task TC_BE_005_CreateJobApplication_WhenJobAtCapacity_ThrowsException()
    {
        // Arrange — job has WorkersNeeded=2 and WorkersAccepted=2 (full)
        var worker = new Worker { Id = Guid.NewGuid(), UserId = _currentUserId, FullName = "Test Worker" };
        var jobPostId = Guid.NewGuid();
        var fullJobPost = new JobPost
        {
            Id = jobPostId,
            FarmerId = _farmerId,
            Title = "Full Job",
            WorkersNeeded = 2,
            WorkersAccepted = 2,   // already at capacity
            StatusId = (int)JobPostStatus.Published,
            JobSkillRequirements = new List<JobSkillRequirement>(),
            Farmer = MakeFarmer()
        };

        _workerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Worker, bool>>>(), null, null))
            .ReturnsAsync(worker);
        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(fullJobPost);

        var request = new CreateJobApplicationRequest { JobPostId = jobPostId };

        var ex = await Assert.ThrowsAsync<Exception>(() => _jobApplicationService.CreateJobApplication(request));
        Assert.Contains("required worker capacity", ex.Message);
    }

    [Fact]
    public async Task TC_BE_005_CreateJobApplication_WhenJobHasOpenSlots_Succeeds()
    {
        // Arrange — job has WorkersNeeded=3, WorkersAccepted=1 (still open)
        var worker = new Worker { Id = Guid.NewGuid(), UserId = _currentUserId, FullName = "Test Worker" };
        var jobPostId = Guid.NewGuid();
        var openJobPost = new JobPost
        {
            Id = jobPostId,
            FarmerId = _farmerId,
            Title = "Open Job",
            WorkersNeeded = 3,
            WorkersAccepted = 1,   // 2 slots still available
            StatusId = (int)JobPostStatus.Published,
            JobSkillRequirements = new List<JobSkillRequirement>(),
            Farmer = MakeFarmer()
        };
        var jobApplication = new JobApplication
        {
            Id = Guid.NewGuid(), WorkerId = worker.Id, JobPostId = jobPostId,
            StatusId = (int)ApplicationStatus.Pending, AppliedAt = DateTime.UtcNow
        };
        var dto = new JobApplicationDTO { Id = jobApplication.Id };

        _workerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Worker, bool>>>(), null, null))
            .ReturnsAsync(worker);
        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<JobPost>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<JobPost, object>>>()))
            .ReturnsAsync(openJobPost);

        var request = new CreateJobApplicationRequest { JobPostId = jobPostId };
        _mapperMock.Setup(m => m.CreateJobApplicationRequestToJobApplication(request)).Returns(jobApplication);
        _mapperMock.Setup(m => m.JobApplicationToJobApplicationDto(jobApplication)).Returns(dto);
        _notificationServiceMock
            .Setup(n => n.CreateAsync(It.IsAny<AgroTemp.Domain.DTO.Notification.CreateNotificationRequest>()))
            .ReturnsAsync(new AgroTemp.Domain.DTO.Notification.NotificationDTO());

        var result = await _jobApplicationService.CreateJobApplication(request);

        Assert.NotNull(result);
        _jobAppRepoMock.Verify(r => r.InsertAsync(jobApplication), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}