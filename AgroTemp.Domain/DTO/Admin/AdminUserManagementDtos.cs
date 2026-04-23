namespace AgroTemp.Domain.DTO.Admin;

public class AdminUserListQuery
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
    public string? Role { get; set; }
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}

public class AdminUserListResponse
{
    public List<AdminUserListItemDto> Data { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
}

public class AdminUserListItemDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public string? AvatarUrl { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminUserDetailDto : AdminUserListItemDto
{
    public string? Address { get; set; }
}
