using AgroTemp.Domain.DTO;
using Microsoft.AspNetCore.Http;

namespace AgroTemp.Service.Implements;

public interface IUserService
{
    Task<UserDTO> GetUserByEmail(string email);
    Task<List<UserDTO>> GetAllUsers();
    Task<UserDTO> GetUserById(Guid id);
    Task<UserDTO> CreateUser(CreateUserRequest request);
    Task<UserDTO> UpdateUser(Guid id, UpdateUserRequest request);
    Task<bool> DeleteUser(Guid id);
    
    Task<FarmerProfileDTO> GetFarmerProfile();
    Task<FarmerProfileDTO> GetFarmerProfileById(Guid id);
    Task<FarmerProfileDTO> UpdateFarmerProfile(UpdateFarmerProfileRequest request);
    
    Task<WorkerProfileDTO> GetWorkerProfile();
    Task<WorkerProfileDTO> GetWorkerProfileById(Guid id);
    Task<WorkerProfileDTO> UpdateWorkerProfile(UpdateWorkerProfileRequest request);

    Task<string> UploadFarmerAvatar(IFormFile file);
    Task<string> UploadWorkerAvatar(IFormFile file);
}