namespace AgroTemp.Domain.DTO.Auth;

public class RefreshResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
