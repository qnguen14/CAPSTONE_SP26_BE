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
}