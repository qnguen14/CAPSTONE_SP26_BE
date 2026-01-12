using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.DTO;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
}