using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Notification;

public class RegisterDeviceTokenRequest
{
    [Required]
    [StringLength(500)]
    public string Token { get; set; }

    [StringLength(256)]
    public string? DeviceName { get; set; }
}
