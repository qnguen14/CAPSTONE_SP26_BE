using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.Mapper;

public interface IMapperlyMapper
{
    // User
    UserDTO UserToUserDto(User user);
    // LoginResponse UserToLoginResponse(User user);
    // User RegisterRequestToUser(RegisterRequest resquest);
    // User RequestDTOToUser(UserRequestDTO request);
    // User UpdateProfileToUser(UpdateProfileRequest request);
}