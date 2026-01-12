using AgroTemp.Domain.DTO.Auth;

namespace AgroTemp.Service.Implements;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
}