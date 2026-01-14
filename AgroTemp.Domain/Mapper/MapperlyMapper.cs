using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace AgroTemp.Domain.Mapper;

[Mapper]
public partial class MapperlyMapper : IMapperlyMapper
{
    [MapProperty(nameof(User.Role), nameof(UserDTO.Role))]
    public partial UserDTO UserToUserDto(User user);
    
    public partial List<UserDTO> UsersToUserDtos(IEnumerable<User> users);
    
    public partial FarmerProfileDTO FarmerProfileToDto(FarmerProfile farmerProfile);
    
    public partial WorkerProfileDTO WorkerProfileToDto(WorkerProfile workerProfile);
    
    // Custom mapping for ExperienceLevel enum to string
    private string MapExperienceLevel(ExperienceLevel level) => level.ToString();
}