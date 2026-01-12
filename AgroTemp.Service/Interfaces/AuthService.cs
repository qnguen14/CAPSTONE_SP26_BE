using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Auth;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AgroTemp.Service.Interfaces;

public class AuthService : BaseService<User>, IAuthService
{
    public AuthService(
        IUnitOfWork<AgroTempDbContext> unitOfWork, 
        IHttpContextAccessor httpContextAccessor, 
        IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
    {
    }

    public Task<LoginResponse> Login(LoginRequest request)
    {
        throw new NotImplementedException();
    }
}