using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgroTemp.Service.Interfaces;

public class UserService : BaseService<User>, IUserService
{
    private readonly IMapperlyMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;
    public UserService(
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper,
        ICloudinaryService cloudinaryService) : base(unitOfWork, httpContextAccessor, mapper)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<UserDTO> GetUserByEmail(string email)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(predicate: u => u.Email == email);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return _mapper.UserToUserDto(user);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
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

    public async Task<UserDTO> GetUserById(Guid id)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(predicate: u => u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return _mapper.UserToUserDto(user);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<UserDTO> CreateUser(CreateUserRequest request)
    {
        try
        {
            var existingEmail = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(predicate: u => u.Email == request.Email);

            if (existingEmail != null)
            {
                throw new Exception("Email already exists");
            }

            var existingPhone = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(predicate: u => u.PhoneNumber == request.PhoneNumber);

            if (existingPhone != null)
            {
                throw new Exception("Phone number already exists");
            }
            var userId = Guid.NewGuid();

            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 0,
                LockedBalance = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var user = new User
            {
                Id = userId,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = (int)request.RoleId,
                Role = request.RoleId,
                CreatedAt = DateTime.UtcNow,
                IsActive = request.IsActive,
                IsVerified = request.IsVerified,
                Wallet = wallet
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.UserToUserDto(user);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<UserDTO> UpdateUser(Guid id, UpdateUserRequest request)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(predicate: u => u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var existingEmail = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(predicate: u => u.Email == request.Email && u.Id != id);

                if (existingEmail != null)
                {
                    throw new Exception("Email already exists");
                }
                user.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
            {
                var existingPhone = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(predicate: u => u.PhoneNumber == request.PhoneNumber && u.Id != id);

                if (existingPhone != null)
                {
                    throw new Exception("Phone number already exists");
                }
                user.PhoneNumber = request.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(request.Address))
            {
                user.Address = request.Address;
            }

            if (request.RoleId.HasValue)
            {
                user.RoleId = request.RoleId.Value;
                user.Role = (UserRole)request.RoleId.Value;
            }

            if (request.IsActive.HasValue)
            {
                user.IsActive = request.IsActive.Value;
            }

            if (request.IsVerified.HasValue)
            {
                user.IsVerified = request.IsVerified.Value;
            }

            _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.UserToUserDto(user);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(predicate: u => u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.IsActive = false;
            _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<FarmerProfileDTO> GetFarmerProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var farmerProfile = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(
                    predicate: fp => fp.UserId == userId,
                    include: fp => fp.Include(x => x.User));

            if (farmerProfile == null)
            {
                throw new Exception("Farmer profile not found");
            }

            return _mapper.FarmerToDto(farmerProfile);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<FarmerProfileDTO> UpdateFarmerProfile(UpdateFarmerProfileRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var farmerProfile = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(
                    predicate: fp => fp.UserId == userId,
                    include: fp => fp.Include(x => x.User));

            if (farmerProfile == null)
            {
                farmerProfile = new Farmer
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ContactName = request.ContactName,
                    AverageRating = 0,
                    TotalJobsPosted = 0,
                    TotalJobsCompleted = 0,
                    AvatarUrl = request.AvatarUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<Farmer>().InsertAsync(farmerProfile);
            }
            else
            {
                farmerProfile.ContactName = request.ContactName;
                farmerProfile.UpdatedAt = DateTime.UtcNow;
                farmerProfile.AvatarUrl = request.AvatarUrl;

                _unitOfWork.GetRepository<Farmer>().UpdateAsync(farmerProfile);
            }

            await _unitOfWork.SaveChangesAsync();

            var farmerProfileWithUser = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(
                    predicate: fp => fp.Id == farmerProfile.Id,
                    include: fp => fp.Include(x => x.User));

            return _mapper.FarmerToDto(farmerProfileWithUser ?? farmerProfile);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<WorkerProfileDTO> GetWorkerProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var workerProfile = await _unitOfWork.GetRepository<Worker>()
                .FirstOrDefaultAsync(
                    predicate: wp => wp.UserId == userId,
                    include: query => query
                        .Include(w => w.User)
                            // .Include(w => w.WorkerSkills)
                            //     .ThenInclude(ws => ws.Skill)
                            );

            if (workerProfile == null)
            {
                throw new Exception("Worker profile not found");
            }

            // DEBUG: Check if User is loaded
            if (workerProfile.User == null)
            {
                throw new Exception($"User not loaded! WorkerId: {workerProfile.Id}, UserId: {workerProfile.UserId}");
            }

            return _mapper.WorkerToDto(workerProfile);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<WorkerProfileDTO> UpdateWorkerProfile(UpdateWorkerProfileRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var workerProfile = await _unitOfWork.GetRepository<Worker>()
                .FirstOrDefaultAsync(
                    predicate: wp => wp.UserId == userId,
                    include: query => query.Include(w => w.User));

            if (workerProfile == null)
            {
                workerProfile = new Worker
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

                await _unitOfWork.GetRepository<Worker>().InsertAsync(workerProfile);
                await _unitOfWork.SaveChangesAsync();

                workerProfile = await _unitOfWork.GetRepository<Worker>()
                    .FirstOrDefaultAsync(
                        predicate: wp => wp.Id == workerProfile.Id,
                        include: query => query
                            .Include(w => w.User)
                                // .Include(w => w.WorkerSkills)
                                //     .ThenInclude(ws => ws.Skill)
                                );
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

                _unitOfWork.GetRepository<Worker>().UpdateAsync(workerProfile);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.WorkerToDto(workerProfile);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<string> UploadFarmerAvatar(IFormFile file)
    {
        try
        {
            var userId = GetCurrentUserId();
            var farmerProfile = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(
                    predicate: fp => fp.UserId == userId,
                    include: null);

            if (farmerProfile == null)
            {
                throw new Exception("Farmer profile not found");
            }

            var imageUrl = await _cloudinaryService.UploadImageAsync(file);

            // Delete old avatar if it exists
            if (!string.IsNullOrEmpty(farmerProfile.AvatarUrl))
            {
                try { await _cloudinaryService.DeleteAsync(farmerProfile.AvatarUrl); } catch { }
            }

            farmerProfile.AvatarUrl = imageUrl;
            farmerProfile.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Farmer>().UpdateAsync(farmerProfile);
            await _unitOfWork.SaveChangesAsync();

            return imageUrl;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<string> UploadWorkerAvatar(IFormFile file)
    {
        try
        {
            var userId = GetCurrentUserId();
            var workerProfile = await _unitOfWork.GetRepository<Worker>()
                .FirstOrDefaultAsync(
                    predicate: wp => wp.UserId == userId,
                    include: null);

            if (workerProfile == null)
            {
                throw new Exception("Worker profile not found");
            }

            var imageUrl = await _cloudinaryService.UploadImageAsync(file);

            // Delete old avatar if it exists
            if (!string.IsNullOrEmpty(workerProfile.AvatarUrl))
            {
                try { await _cloudinaryService.DeleteAsync(workerProfile.AvatarUrl); } catch { }
            }

            workerProfile.AvatarUrl = imageUrl;
            workerProfile.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Worker>().UpdateAsync(workerProfile);
            await _unitOfWork.SaveChangesAsync();

            return imageUrl;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
