using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Admin;
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
                .FirstOrDefaultAsync(
                    predicate: u => u.Email == email,
                    include: u => u.Include(x => x.Farmer).Include(x => x.Worker));

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
                    include: u => u.Include(x => x.Farmer).Include(x => x.Worker),
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
                .FirstOrDefaultAsync(
                    predicate: u => u.Id == id,
                    include: u => u.Include(x => x.Farmer).Include(x => x.Worker));

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

    public async Task<AdminUserListResponse> GetAdminUsers(AdminUserListQuery query)
    {
        try
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var limit = query.Limit <= 0 ? 20 : Math.Min(query.Limit, 100);

            var usersQuery = BuildAdminUsersQuery();

            if (!string.IsNullOrWhiteSpace(query.Role))
            {
                var normalizedRole = query.Role.Trim().ToLowerInvariant();
                usersQuery = normalizedRole switch
                {
                    "farmer" => usersQuery.Where(x => x.RoleId == (int)UserRole.Farmer),
                    "worker" => usersQuery.Where(x => x.RoleId == (int)UserRole.Worker),
                    "admin" => usersQuery.Where(x => x.RoleId == (int)UserRole.Admin),
                    _ => throw new Exception("Invalid role filter. Supported values: farmer, worker, admin")
                };
            }

            if (query.IsActive.HasValue)
            {
                usersQuery = usersQuery.Where(x => x.IsActive == query.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                usersQuery = usersQuery.Where(x =>
                    x.Email.ToLower().Contains(search) ||
                    x.PhoneNumber.ToLower().Contains(search) ||
                    (x.Worker != null && x.Worker.FullName.ToLower().Contains(search)) ||
                    (x.Farmer != null && x.Farmer.ContactName.ToLower().Contains(search)));
            }

            var total = await usersQuery.CountAsync();
            var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);

            var data = await usersQuery
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new AdminUserListItemDto
                {
                    Id = x.Id,
                    FullName = x.Worker != null ? x.Worker.FullName : (x.Farmer != null ? x.Farmer.ContactName : x.Email),
                    Email = x.Email,
                    Role = GetRoleName(x.RoleId),
                    IsActive = x.IsActive,
                    IsVerified = x.IsVerified,
                    AvatarUrl = x.Worker != null ? x.Worker.AvatarUrl : (x.Farmer != null ? x.Farmer.AvatarUrl : null),
                    PhoneNumber = x.PhoneNumber,
                    Rating = x.Worker != null ? x.Worker.AverageRating : (x.Farmer != null ? x.Farmer.AverageRating : null),
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return new AdminUserListResponse
            {
                Data = data,
                Total = total,
                Page = page,
                Limit = limit,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<AdminUserDetailDto> GetAdminUserById(Guid id)
    {
        try
        {
            var user = await GetAdminUserEntityById(id);
            return MapAdminUserDetail(user);
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
                .FirstOrDefaultAsync(
                    predicate: u => u.Id == id,
                    include: u => u.Include(x => x.Farmer).Include(x => x.Worker));

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

    public async Task<AdminUserDetailDto> UpdateAdminUser(Guid id, UpdateUserRequest request)
    {
        try
        {
            var user = await GetAdminUserEntityById(id);

            await ValidateAndUpdateCoreUserFields(id, user, request);
            ApplyAddressUpdate(user, request.Address);

            _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var updatedUser = await GetAdminUserEntityById(id);
            return MapAdminUserDetail(updatedUser);
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

    public async Task<bool> DeleteAdminUser(Guid id)
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

    private IQueryable<User> BuildAdminUsersQuery()
    {
        return _unitOfWork.Context.Set<User>()
            .AsNoTracking()
            .Include(x => x.Worker)
            .Include(x => x.Farmer);
    }

    private async Task<User> GetAdminUserEntityById(Guid id)
    {
        var user = await _unitOfWork.Context.Set<User>()
            .Include(x => x.Worker)
            .Include(x => x.Farmer)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        return user;
    }

    private async Task ValidateAndUpdateCoreUserFields(Guid id, User user, UpdateUserRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            var existingEmail = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(predicate: u => u.Email == request.Email && u.Id != id);

            if (existingEmail != null)
            {
                throw new Exception("Email already exists");
            }

            user.Email = request.Email;
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
        {
            var existingPhone = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(predicate: u => u.PhoneNumber == request.PhoneNumber && u.Id != id);

            if (existingPhone != null)
            {
                throw new Exception("Phone number already exists");
            }

            user.PhoneNumber = request.PhoneNumber;
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
    }

    private static void ApplyAddressUpdate(User user, string? address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return;
        }

        if (user.Worker != null)
        {
            user.Worker.PrimaryLocation = address;
            user.Worker.UpdatedAt = DateTime.UtcNow;
            return;
        }

        if (user.Farmer != null)
        {
            user.Farmer.Address = address;
            user.Farmer.UpdatedAt = DateTime.UtcNow;
        }
    }

    private static AdminUserDetailDto MapAdminUserDetail(User user)
    {
        return new AdminUserDetailDto
        {
            Id = user.Id,
            FullName = user.Worker != null ? user.Worker.FullName : (user.Farmer != null ? user.Farmer.ContactName : user.Email),
            Email = user.Email,
            Role = GetRoleName(user.RoleId),
            IsActive = user.IsActive,
            IsVerified = user.IsVerified,
            AvatarUrl = user.Worker != null ? user.Worker.AvatarUrl : (user.Farmer != null ? user.Farmer.AvatarUrl : null),
            PhoneNumber = user.PhoneNumber,
            Rating = user.Worker != null ? user.Worker.AverageRating : (user.Farmer != null ? user.Farmer.AverageRating : null),
            CreatedAt = user.CreatedAt,
            Address = user.Worker != null ? user.Worker.PrimaryLocation : (user.Farmer != null ? user.Farmer.Address : null)
        };
    }

    private static string GetRoleName(int roleId)
    {
        return roleId switch
        {
            (int)UserRole.Admin => "admin",
            (int)UserRole.Farmer => "farmer",
            (int)UserRole.Worker => "worker",
            _ => "unknown"
        };
    }

    public async Task<FarmerProfileDTO> GetFarmerProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var farmerProfile = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(
                    predicate: fp => fp.UserId == userId,
                    include: fp => fp.Include(x => x.User)
                                     .Include(x => x.Farms));

            if (farmerProfile == null)
            {
                throw new Exception("Farmer profile not found");
            }

            var farms = farmerProfile.Farms.OrderBy(f => f.CreatedAt).ToList();
            var mainFarm = farms.FirstOrDefault(predicate: f => f.IsPrimary) ?? farms.FirstOrDefault();
            var response = _mapper.FarmerToDto(farmerProfile);
            response.MainFarmId = mainFarm?.Id ?? Guid.Empty;

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<FarmerProfileDTO> GetFarmerProfileById(Guid id)
    {
        try
        {
            var farmerProfile = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(
                    predicate: fp => fp.UserId == id,
                    include: fp => fp.Include(x => x.User)
                                     .Include(x => x.Farms));

            if (farmerProfile == null)
            {
                throw new Exception("Farmer profile not found");
            }

            var farms = farmerProfile.Farms.OrderBy(f => f.CreatedAt).ToList();
            var mainFarm = farms.FirstOrDefault(predicate: f => f.IsPrimary) ?? farms.FirstOrDefault();
            var response = _mapper.FarmerToDto(farmerProfile);
            response.MainFarmId = mainFarm?.Id ?? Guid.Empty;

            return response;
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
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
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
                farmerProfile.Address = request.Address;
                farmerProfile.DateOfBirth = request.DateOfBirth;
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
                        .Include(w => w.WorkerSkills)
                            .ThenInclude(ws => ws.Skill)
                            );

            if (workerProfile == null)
            {
                throw new Exception("Worker profile not found");
            }

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

    public async Task<WorkerProfileDTO> GetWorkerProfileById(Guid id)
    {
        try
        {
            var workerProfile = await _unitOfWork.GetRepository<Worker>()
                .FirstOrDefaultAsync(
                    predicate: wp => wp.UserId == id,
                    include: query => query
                        .Include(w => w.User)
                        .Include(w => w.WorkerSkills)
                            .ThenInclude(ws => ws.Skill)
                            );

            if (workerProfile == null)
            {
                throw new Exception("Worker profile not found");
            }

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
            Console.WriteLine($"[DEBUG] UpdateWorkerProfile: Processing UserId: {userId}");

            var workerProfile = await _unitOfWork.GetRepository<Worker>()
                .FirstOrDefaultAsync(
                    predicate: wp => wp.UserId == userId);

            if (workerProfile == null)
            {
                Console.WriteLine($"[DEBUG] UpdateWorkerProfile: Profile not found, creating new for UserId: {userId}");
                workerProfile = new Worker
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FullName = request.FullName,
                    DateOfBirth = DateOnly.Parse(request.DateOfBirth),
                    PrimaryLocation = request.PrimaryLocation,
                    TravelRadiusKmPreference = request.TravelRadiusKmPreference,
                    ExperienceLevelId = request.ExperienceLevelId,
                    AvailabilitySchedule = request.AvailabilitySchedule,
                    AvatarUrl = request.AvatarUrl,
                    AverageRating = 0,
                    TotalJobsCompleted = 0,
                    GenderId = request.GenderId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    WorkerSkills = request.SkillIds?.Distinct().Select(skillId => new WorkerSkill
                    {
                        Id = Guid.NewGuid(),
                        SkillId = skillId,
                        ProficiencyLevelId = (int)ProficiencyLevel.Beginner,
                        YearsExperience = 0
                    }).ToList() ?? new List<WorkerSkill>()
                };

                await _unitOfWork.GetRepository<Worker>().InsertAsync(workerProfile);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"[DEBUG] UpdateWorkerProfile: Found profile for UserId: {userId}. WorkerId: {workerProfile.Id}");
                
                await _unitOfWork.ExecuteInTransactionAsync(async () => {
                    workerProfile.FullName = request.FullName;
                    workerProfile.DateOfBirth = DateOnly.Parse(request.DateOfBirth);
                    workerProfile.PrimaryLocation = request.PrimaryLocation;
                    workerProfile.TravelRadiusKmPreference = request.TravelRadiusKmPreference;
                    workerProfile.ExperienceLevelId = request.ExperienceLevelId;
                    workerProfile.AvailabilitySchedule = request.AvailabilitySchedule;
                    workerProfile.AvatarUrl = request.AvatarUrl;
                    workerProfile.GenderId = request.GenderId;
                    workerProfile.UpdatedAt = DateTime.UtcNow;

                    if (request.SkillIds != null)
                    {
                        var requestedSkillIds = request.SkillIds.Distinct().ToList();
                        var existingSkills = await _unitOfWork.GetRepository<WorkerSkill>()
                            .GetListAsync(predicate: ws => ws.WorkerId == workerProfile.Id);

                        var toRemove = existingSkills.Where(es => !requestedSkillIds.Contains(es.SkillId)).ToList();
                        if (toRemove.Any())
                        {
                            Console.WriteLine($"[DEBUG] UpdateWorkerProfile: Removing {toRemove.Count} skills for WorkerId: {workerProfile.Id}");
                            _unitOfWork.GetRepository<WorkerSkill>().DeleteRangeAsync(toRemove);
                        }

                        // Identify skills to add
                        var currentSkillIds = existingSkills.Select(es => es.SkillId).ToList();
                        var toAdd = requestedSkillIds.Where(id => !currentSkillIds.Contains(id)).ToList();

                        if (toAdd.Any())
                        {
                            Console.WriteLine($"[DEBUG] UpdateWorkerProfile: Adding {toAdd.Count} new skills for WorkerId: {workerProfile.Id}");
                            var newSkills = toAdd.Select(skillId => new WorkerSkill
                            {
                                Id = Guid.NewGuid(),
                                WorkerId = workerProfile.Id,
                                SkillId = skillId,
                                ProficiencyLevelId = (int)ProficiencyLevel.Beginner,
                                YearsExperience = 0
                            }).ToList();
                            await _unitOfWork.GetRepository<WorkerSkill>().InsertRangeAsync(newSkills);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    Console.WriteLine($"[DEBUG] UpdateWorkerProfile: Atomic Transaction committed for WorkerId: {workerProfile.Id}");
                });
            }

            var finalProfile = await _unitOfWork.GetRepository<Worker>()
                .FirstOrDefaultAsync(
                    predicate: wp => wp.Id == workerProfile.Id,
                    include: query => query
                        .Include(w => w.User)
                        .Include(w => w.WorkerSkills)
                            .ThenInclude(ws => ws.Skill)
                            );

            return _mapper.WorkerToDto(finalProfile ?? workerProfile);
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
