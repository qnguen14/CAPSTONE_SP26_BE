using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Auth;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
