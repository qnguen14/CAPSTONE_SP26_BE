using AgroTemp.Domain.DTO;

namespace AgroTemp.Service.Implements;

public interface IUserService
{
    Task<UserDTO> GetUserByEmail(string email);
    Task<List<UserDTO>> GetAllUsers();
    Task<UserDTO> GetUserById(string id);
    Task<UserDTO> CreateUser(UserDTO userDto);
    Task<UserDTO> UpdateUser(string id, UserDTO userDto);
    Task<bool> DeleteUser(string id);
}