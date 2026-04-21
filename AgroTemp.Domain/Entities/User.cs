using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum UserRole
{
    Admin = 1,
    Farmer = 2,
    Worker = 3
}

[Table("User")]
public class User
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("email")]
    [StringLength(256)]
    public string Email { get; set; }

    [Required]
    [Column("phone_number")]
    [StringLength(10)]
    public string PhoneNumber { get; set; }

    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; }

    [Required]
    [Column("role_id")]
    public int RoleId { get; set; }
    
    [NotMapped]
    public UserRole Role 
    { 
        get => (UserRole)RoleId;
        set => RoleId = (int)value;
    }
    
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Required]
    [Column("is_verified")]
    public bool IsVerified { get; set; }

    [Column("warning_count")]
    public int WarningCount { get; set; } = 0;

    [Column("last_warned_at")]
    public DateTime? LastWarnedAt { get; set; }

    [Column("verification_token")]
    public string? VerificationToken { get; set; }

    [Column("verification_token_expires_at")]
    public DateTime? VerificationTokenExpiresAt { get; set; }

    [Column("password_reset_token")]
    public string? PasswordResetToken { get; set; }

    [Column("password_reset_token_expires_at")]
    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    // Navigation properties
    public virtual Worker? Worker { get; set; }
    public virtual Farmer? Farmer { get; set; }
    
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
    public virtual ICollection<ChatMessage> ReceivedMessages { get; set; } = new List<ChatMessage>();
    public virtual ICollection<DeviceToken> DeviceTokens { get; set; } = new List<DeviceToken>();
    
    public virtual ICollection<Rating> GivenRatings { get; set; } = new List<Rating>();
    public virtual ICollection<Rating> ReceivedRatings { get; set; } = new List<Rating>();

    public virtual Wallet? Wallet { get; set; }
}