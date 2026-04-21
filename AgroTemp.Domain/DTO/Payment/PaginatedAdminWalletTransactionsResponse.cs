using System.Collections.Generic;

namespace AgroTemp.Domain.DTO.Payment;

public class WalletTransactionItemForAdmin
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public Guid? JobDetailId { get; set; }
    public string? UserName { get; set; }
    public decimal Amount { get; set; }
    public string? Type { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class PaginatedAdminWalletTransactionsResponse
{
    public IEnumerable<WalletTransactionItemForAdmin> Items { get; set; } = new List<WalletTransactionItemForAdmin>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
}
