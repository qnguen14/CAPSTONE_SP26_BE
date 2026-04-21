namespace AgroTemp.Domain.DTO.Payment;

public class AdminWalletStatsResponse
{
    public SystemBalanceDto SystemBalance { get; set; } = new SystemBalanceDto();
    public PayosTodayDto PayosToday { get; set; } = new PayosTodayDto();
}

public class SystemBalanceDto
{
    public decimal Total { get; set; }
    public decimal Locked { get; set; }
    public decimal Available { get; set; }
    public decimal ChangeToday { get; set; }
}

public class PayosTodayDto
{
    public decimal DepositAmount { get; set; }
    public decimal WithdrawAmount { get; set; }
    public int DepositCount { get; set; }
    public int WithdrawCount { get; set; }
    public int TotalTransactions { get; set; }
    public decimal NetFlow { get; set; }
}
