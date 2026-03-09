using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Auth;

public class VerifyEmailRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Otp { get; set; }
}
