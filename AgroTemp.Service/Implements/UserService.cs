using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AgroTemp.Service.Interfaces;

public class UserService : BaseService<User>, IUserService
{
    private readonly IMapperlyMapper _mapper;
    public UserService(
        IUnitOfWork<AgroTempDbContext> unitOfWork, 
        IHttpContextAccessor httpContextAccessor, 
        IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public Task<UserDTO> GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<List<UserDTO>> GetAllUsers()
    {
        try
        {
            var users = await _unitOfWork.GetRepository<User>()
                .GetListAsync(
                    predicate: null,
                    include: null,
                    orderBy: u => u.OrderBy(x => x.Email));

            if (users == null || !users.Any())
            {
                return null;
            }
            
            var result = _mapper.UsersToUserDtos(users);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<UserDTO> GetUserById(string id)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO> CreateUser(UserDTO userDto)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO> UpdateUser(string id, UserDTO userDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUser(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<FarmerProfileDTO> GetFarmerProfile(Guid userId)
    {
        try
        {
            var farmerProfile = await _unitOfWork.GetRepository<FarmerProfile>()
                .FirstOrDefaultAsync(
                    predicate: fp => fp.UserId == userId,
                    include: null);

            if (farmerProfile == null)
            {
                throw new Exception("Farmer profile not found");
            }

            return _mapper.FarmerProfileToDto(farmerProfile);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<FarmerProfileDTO> UpdateFarmerProfile(Guid userId, UpdateFarmerProfileRequest request)
    {
        try
        {
            var farmerProfile = await _unitOfWork.GetRepository<FarmerProfile>()
                .FirstOrDefaultAsync(
                    predicate: fp => fp.UserId == userId,
                    include: null);

            if (farmerProfile == null)
            {
                // Profile doesn't exist - create it (first-time setup)
                farmerProfile = new FarmerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    OrganizationName = request.OrganizationName,
                    ContactName = request.ContactName,
                    ContactNumber = request.ContactNumber,
                    CooperativeAffiliation = request.CooperativeAffiliation,
                    FarmType = request.FarmType,
                    AverageRating = 0,
                    TotalJobsPosted = 0,
                    TotalJobsCompleted = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<FarmerProfile>().InsertAsync(farmerProfile);
            }
            else
            {
                // Profile exists - update it
                farmerProfile.OrganizationName = request.OrganizationName;
                farmerProfile.ContactName = request.ContactName;
                farmerProfile.ContactNumber = request.ContactNumber;
                farmerProfile.CooperativeAffiliation = request.CooperativeAffiliation;
                farmerProfile.FarmType = request.FarmType;
                farmerProfile.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<FarmerProfile>().UpdateAsync(farmerProfile);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.FarmerProfileToDto(farmerProfile);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<WorkerProfileDTO> GetWorkerProfile(Guid userId)
    {
        try
        {
            var workerProfile = await _unitOfWork.GetRepository<WorkerProfile>()
                .FirstOrDefaultAsync(
                    predicate: wp => wp.UserId == userId,
                    include: null);

            if (workerProfile == null)
            {
                throw new Exception("Worker profile not found");
            }

            return _mapper.WorkerProfileToDto(workerProfile);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<WorkerProfileDTO> UpdateWorkerProfile(Guid userId, UpdateWorkerProfileRequest request)
    {
        try
        {
            var workerProfile = await _unitOfWork.GetRepository<WorkerProfile>()
                .FirstOrDefaultAsync(
                    predicate: wp => wp.UserId == userId,
                    include: null);

            if (workerProfile == null)
            {
                // Profile doesn't exist - create it (first-time setup)
                workerProfile = new WorkerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FullName = request.FullName,
                    AgeRange = request.AgeRange,
                    PrimaryLocation = request.PrimaryLocation,
                    TravelRadiusKmPreference = request.TravelRadiusKmPreference,
                    ExperienceLevelId = request.ExperienceLevelId,
                    AvailabilitySchedule = request.AvailabilitySchedule,
                    AvatarUrl = request.AvatarUrl,
                    AverageRating = 0,
                    TotalJobsCompleted = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<WorkerProfile>().InsertAsync(workerProfile);
            }
            else
            {
                // Profile exists - update it
                workerProfile.FullName = request.FullName;
                workerProfile.AgeRange = request.AgeRange;
                workerProfile.PrimaryLocation = request.PrimaryLocation;
                workerProfile.TravelRadiusKmPreference = request.TravelRadiusKmPreference;
                workerProfile.ExperienceLevelId = request.ExperienceLevelId;
                workerProfile.AvailabilitySchedule = request.AvailabilitySchedule;
                workerProfile.AvatarUrl = request.AvatarUrl;
                workerProfile.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<WorkerProfile>().UpdateAsync(workerProfile);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.WorkerProfileToDto(workerProfile);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}