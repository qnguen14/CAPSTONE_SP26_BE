using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO;

public class UpdateUserRequest
{
    [EmailAddress]
    public string? Email { get; set; }
    
    [Phone]
    [StringLength(10)]
    public string? PhoneNumber { get; set; }
    
    public string? Address { get; set; }
    
    [Range(1, 3)]
    public int? RoleId { get; set; }
    
    public bool? IsActive { get; set; }
    
    public bool? IsVerified { get; set; }
}
