using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Rating;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

public class RatingServiceTest
{
    private readonly Mock<IUnitOfWork<AgroTempDbContext>> _unitOfWorkMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IMapperlyMapper> _mapperMock;
    private readonly Mock<IGenericRepository<Rating>> _ratingRepoMock;
    private readonly Mock<IGenericRepository<User>> _userRepoMock;
    private readonly Mock<IGenericRepository<JobPost>> _jobPostRepoMock;
    private readonly Mock<IGenericRepository<Worker>> _workerRepoMock;
    private readonly Mock<IGenericRepository<Farmer>> _farmerRepoMock;
    private readonly RatingService _service;

    private readonly Guid _raterUserId = Guid.NewGuid();
    private readonly Guid _rateeUserId = Guid.NewGuid();
    private readonly Guid _jobPostId = Guid.NewGuid();

    public RatingServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork<AgroTempDbContext>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _mapperMock = new Mock<IMapperlyMapper>();
        _ratingRepoMock = new Mock<IGenericRepository<Rating>>();
        _userRepoMock = new Mock<IGenericRepository<User>>();
        _jobPostRepoMock = new Mock<IGenericRepository<JobPost>>();
        _workerRepoMock = new Mock<IGenericRepository<Worker>>();
        _farmerRepoMock = new Mock<IGenericRepository<Farmer>>();

        // Authenticated rater
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _raterUserId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var httpContext = new DefaultHttpContext { User = new System.Security.Claims.ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Repositories
        _unitOfWorkMock.Setup(u => u.GetRepository<Rating>()).Returns(_ratingRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<User>()).Returns(_userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<JobPost>()).Returns(_jobPostRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<Worker>()).Returns(_workerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<Farmer>()).Returns(_farmerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new RatingService(
            _unitOfWorkMock.Object,
            _httpContextAccessorMock.Object,
            _mapperMock.Object);
    }

    // Helpers

    private User MakeRateeUser() => new User
    {
        Id = _rateeUserId,
        Email = "ratee@test.com",
        PhoneNumber = "0123456789",
        PasswordHash = "hash",
        RoleId = (int)UserRole.Worker,
        CreatedAt = DateTime.UtcNow,
        IsActive = true,
        IsVerified = true
    };

    private JobPost MakeJobPost() => new JobPost
    {
        Id = _jobPostId,
        FarmerId = Guid.NewGuid(),
        Title = "Harvest Help",
        Description = "Need workers",
        Address = "Farm",
        WageAmount = 100,
        WorkersNeeded = 2,
        StatusId = (int)JobPostStatus.Completed,
        JobTypeId = (int)JobType.PerJob,
        JobSkillRequirements = new List<JobSkillRequirement>()
    };

    private Rating MakeRating(int score = 4) => new Rating
    {
        Id = Guid.NewGuid(),
        RaterId = _raterUserId,
        RateeId = _rateeUserId,
        JobPostId = _jobPostId,
        RatingScore = score,
        TypeId = (int)RatingType.FarmerToWorker,
        CreatedAt = DateTime.UtcNow
    };

    private RatingDTO MakeRatingDTO(Rating rating) => new RatingDTO
    {
        Id = rating.Id,
        RaterId = rating.RaterId,
        RateeId = rating.RateeId,
        JobPostId = rating.JobPostId,
        RatingScore = rating.RatingScore,
        TypeId = rating.TypeId,
        CreatedAt = rating.CreatedAt
    };

    private (Rating rating, RatingDTO dto, CreateRatingRequest request) SetupValidFlow(
        int score = 4,
        int typeId = (int)RatingType.FarmerToWorker)
    {
        var request = new CreateRatingRequest
        {
            RateeId = _rateeUserId,
            JobPostId = _jobPostId,
            RatingScore = score,
            ReviewText = "Great work!",
            TypeId = typeId,
            CreatedAt = DateTime.UtcNow
        };

        var rating = MakeRating(score);
        rating.TypeId = typeId;
        var dto = MakeRatingDTO(rating);

        // Ratee exists
        _userRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), null, null))
            .ReturnsAsync(MakeRateeUser());

        // Job post exists
        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, null))
            .ReturnsAsync(MakeJobPost());

        // No duplicate rating → then return the created rating on the second call
        _ratingRepoMock
            .SetupSequence(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<Rating>,
                    Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Rating, object>>>()))
            .ReturnsAsync((Rating)null!)   // duplicate check → no existing rating
            .ReturnsAsync(rating);         // final fetch of created rating

        // Average rating calculation
        _ratingRepoMock
            .Setup(r => r.GetListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, bool>>>(),
                null, null, null))
            .ReturnsAsync(new List<Rating> { rating });

        _mapperMock.Setup(m => m.CreateRatingRequestToRating(request)).Returns(rating);
        _mapperMock.Setup(m => m.RatingToRatingDto(rating)).Returns(dto);

        return (rating, dto, request);
    }

    // Valid FarmerToWorker rating

    [Fact]
    public async Task UTCID32_CreateRating_Valid_FarmerToWorker_ReturnsRatingDTO()
    {
        var (rating, dto, request) = SetupValidFlow(score: 4, typeId: (int)RatingType.FarmerToWorker);

        var worker = new Worker
        {
            Id = Guid.NewGuid(), UserId = _rateeUserId,
            FullName = "Test Worker", AverageRating = 0
        };
        _workerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Worker, bool>>>(), null, null))
            .ReturnsAsync(worker);

        var result = await _service.CreateRating(request);

        Assert.NotNull(result);
        Assert.Equal(dto.RatingScore, result.RatingScore);
        Assert.Equal((int)RatingType.FarmerToWorker, result.TypeId);
        _ratingRepoMock.Verify(r => r.InsertAsync(rating), Times.Once);
        _workerRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Worker>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    // Valid WorkerToFarmer rating

    [Fact]
    public async Task UTCID33_CreateRating_Valid_WorkerToFarmer_ReturnsRatingDTO()
    {
        var (rating, dto, request) = SetupValidFlow(score: 5, typeId: (int)RatingType.WorkerToFarmer);

        var farmer = new Farmer
        {
            Id = Guid.NewGuid(), UserId = _rateeUserId,
            ContactName = "Test Farmer", Address = "Farm Rd", AverageRating = 0
        };
        _farmerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Farmer, bool>>>(), null, null))
            .ReturnsAsync(farmer);

        var result = await _service.CreateRating(request);

        Assert.NotNull(result);
        Assert.Equal((int)RatingType.WorkerToFarmer, result.TypeId);
        _ratingRepoMock.Verify(r => r.InsertAsync(rating), Times.Once);
        _farmerRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Farmer>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    // Ratee not found

    [Fact]
    public async Task UTCID34_CreateRating_WhenRateeNotFound_ThrowsException()
    {
        var request = new CreateRatingRequest
        {
            RateeId = Guid.NewGuid(),
            JobPostId = _jobPostId,
            RatingScore = 4,
            TypeId = (int)RatingType.FarmerToWorker
        };

        _userRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), null, null))
            .ReturnsAsync((User)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateRating(request));
        Assert.Contains("does not exist", ex.Message);
    }

    // Job post not found

    [Fact]
    public async Task UTCID35_CreateRating_WhenJobPostNotFound_ThrowsException()
    {
        var request = new CreateRatingRequest
        {
            RateeId = _rateeUserId,
            JobPostId = Guid.NewGuid(),
            RatingScore = 4,
            TypeId = (int)RatingType.FarmerToWorker
        };

        _userRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), null, null))
            .ReturnsAsync(MakeRateeUser());

        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, null))
            .ReturnsAsync((JobPost)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateRating(request));
        Assert.Contains("does not exist", ex.Message);
    }

    // Duplicate rating

    [Fact]
    public async Task UTCID36_CreateRating_WhenDuplicateRating_ThrowsException()
    {
        var request = new CreateRatingRequest
        {
            RateeId = _rateeUserId,
            JobPostId = _jobPostId,
            RatingScore = 3,
            TypeId = (int)RatingType.FarmerToWorker
        };

        _userRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), null, null))
            .ReturnsAsync(MakeRateeUser());

        _jobPostRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<JobPost, bool>>>(), null, null))
            .ReturnsAsync(MakeJobPost());

        _ratingRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Rating, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<Rating>,
                    Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Rating, object>>>()))
            .ReturnsAsync(MakeRating());

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateRating(request));
        Assert.Contains("already submitted a rating", ex.Message);
    }

    // Boundary — minimum score (1)

    [Fact]
    public async Task UTCID37_CreateRating_WithMinimumScore_1_Succeeds()
    {
        var (rating, dto, request) = SetupValidFlow(score: 1, typeId: (int)RatingType.FarmerToWorker);

        _workerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Worker, bool>>>(), null, null))
            .ReturnsAsync(new Worker
            {
                Id = Guid.NewGuid(), UserId = _rateeUserId,
                FullName = "Worker", AverageRating = 0
            });

        var result = await _service.CreateRating(request);

        Assert.NotNull(result);
        Assert.Equal(1, result.RatingScore);
    }

    // Boundary — maximum score (5)

    [Fact]
    public async Task UTCID38_CreateRating_WithMaximumScore_5_Succeeds()
    {
        var (rating, dto, request) = SetupValidFlow(score: 5, typeId: (int)RatingType.FarmerToWorker);

        _workerRepoMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Worker, bool>>>(), null, null))
            .ReturnsAsync(new Worker
            {
                Id = Guid.NewGuid(), UserId = _rateeUserId,
                FullName = "Worker", AverageRating = 0
            });

        var result = await _service.CreateRating(request);

        Assert.NotNull(result);
        Assert.Equal(5, result.RatingScore);
    }
}
