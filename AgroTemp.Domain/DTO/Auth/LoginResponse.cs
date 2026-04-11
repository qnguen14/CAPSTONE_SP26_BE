namespace AgroTemp.Domain.DTO.Auth;

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public Guid UserId { get; set; }
    public Boolean IsVerified { get; set; }
}