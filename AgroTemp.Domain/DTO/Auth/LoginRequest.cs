using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Auth;

public class LoginRequest : IValidatableObject
{
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    [Required]
    public string Password { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(PhoneNumber))
        {
            yield return new ValidationResult(
                "Either Email or Phone Number must be entered",
                new[] { nameof(Email), nameof(PhoneNumber) });
        }

        if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(PhoneNumber))
        {
            yield return new ValidationResult(
                "Provide either Email or PhoneNumber, not both",
                new[] { nameof(Email), nameof(PhoneNumber) });
        }
    }
}