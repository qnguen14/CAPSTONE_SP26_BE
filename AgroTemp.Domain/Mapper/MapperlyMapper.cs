using AgroTemp.Domain.DTO;
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
}