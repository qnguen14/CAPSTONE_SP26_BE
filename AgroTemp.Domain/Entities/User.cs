using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

public enum UserRole
{
    Admin = 1,
    Farmer = 2,
    Worker = 3
}

[Table("Users")]
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
    [Column("address")]
    public string Address { get; set; }

    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; }

    [Required]
    [Column("role_id")]
    public int RoleId { get; set; }
    public virtual UserRole Role { get; set; }
    
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Required]
    [Column("is_verified")]
    public bool IsVerified { get; set; }

    // Navigation properties
    public virtual WorkerProfile? WorkerProfile { get; set; }
    public virtual FarmerProfile? FarmerProfile { get; set; }
}