using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Auth;

public class GoogleLoginRequest
{
    [Required]
    public string GoogleToken { get; set; } = string.Empty;
    
    // Optional: If you want to allow specifying role during first-time Google login
    public int? RoleId { get; set; }
}
