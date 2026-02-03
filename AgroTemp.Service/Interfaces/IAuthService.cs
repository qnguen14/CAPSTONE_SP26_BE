using AgroTemp.Domain.DTO.Auth;

namespace AgroTemp.Service.Implements;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
    Task<LoginResponse> Register(RegisterRequest request);
    Task<LoginResponse> GoogleLogin(GoogleLoginRequest request);
    Task<bool> VerifyEmail(VerifyEmailRequest request);
}