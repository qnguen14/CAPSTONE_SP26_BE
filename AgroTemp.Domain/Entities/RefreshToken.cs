using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Refresh_Tokens")]
public class RefreshToken
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("token")]
    public string Token { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }
}
