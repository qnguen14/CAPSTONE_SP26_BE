using AgroTemp.Domain.DTO.Auth;

namespace AgroTemp.Service.Implements;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
    Task<LoginResponse> Register(RegisterRequest request);
    Task<LoginResponse> GoogleLogin(GoogleLoginRequest request);
    Task ForgotPassword(ForgotPasswordRequest request);
    Task<bool> ResetPassword(ResetPasswordRequest request);
    Task Logout(string token);
}