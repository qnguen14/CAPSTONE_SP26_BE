using System.Collections.Generic;

namespace AgroTemp.Domain.DTO.Payment;

public class WalletItemForAdmin
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public decimal Balance { get; set; }
    public decimal LockedBalance { get; set; }
}

public class PaginatedAdminWalletsResponse
{
    public IEnumerable<WalletItemForAdmin> Items { get; set; } = new List<WalletItemForAdmin>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
}
