namespace AgroTemp.Domain.DTO.Payment;

public class WithdrawalAccountBalanceResponse
{
    public long Balance { get; set; }
    public long AvailableBalance { get; set; }
    public string? Currency { get; set; }
}
