namespace AgroTemp.Domain.DTO.Auth;

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Email { get; set; }
}