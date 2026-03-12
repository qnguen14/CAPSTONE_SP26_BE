using System.Security.Claims;
using AgroTemp.Domain.Context;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AgroTemp.Service.Base;

public abstract class BaseService<T> where T : class
{
    protected IUnitOfWork<AgroTempDbContext> _unitOfWork;
    protected IHttpContextAccessor _httpContextAccessor;
    protected IMapperlyMapper _mapper;
        
    public BaseService(IUnitOfWork<AgroTempDbContext> unitOfWork, IHttpContextAccessor httpContextAccessor, IMapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }
        
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            // throw new UnauthorizedException("User ID not found in token");
            return Guid.Empty;
        }
        return userId;
    }
        
    protected string GetCurrentUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
               // ?? throw new UnauthorizedException("User email not found in token"); 
               ?? string.Empty;
    }
        
    protected string GetCurrentUserRole()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value
               // ?? throw new UnauthorizedException("User role not found in token");
               ?? string.Empty;
    }
}