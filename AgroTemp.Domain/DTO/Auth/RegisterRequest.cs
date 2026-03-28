using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Auth;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
    
    [Required]
    [Phone]
    [StringLength(10)]
    public string PhoneNumber { get; set; }
    
    [Required]
    [Range(1, 3)]
    public int RoleId { get; set; }
}
