using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Auth;

public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Otp { get; set; }

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; }
}
