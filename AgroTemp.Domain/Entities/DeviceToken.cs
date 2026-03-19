using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum DevicePlatform
{
    Android = 1
}

[Table("DeviceToken")]
public class DeviceToken
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    [Column("user_id")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    [Required]
    [Column("expo_push_token")]
    public string ExpoPushToken { get; set; }

    [Required]
    [Column("platform")]
    public DevicePlatform Platform { get; set; } = DevicePlatform.Android;

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("last_used_at")]
    public DateTime LastUsedAt { get; set; }

    [Column("device_name")]
    [StringLength(256)]
    public string? DeviceName { get; set; }
}