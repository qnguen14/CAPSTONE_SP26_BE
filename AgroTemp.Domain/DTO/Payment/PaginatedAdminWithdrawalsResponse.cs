using System.Collections.Generic;

namespace AgroTemp.Domain.DTO.Payment;

public class PaginatedAdminWithdrawalsResponse
{
    public IEnumerable<WithdrawalResponse> Items { get; set; } = new List<WithdrawalResponse>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
}
