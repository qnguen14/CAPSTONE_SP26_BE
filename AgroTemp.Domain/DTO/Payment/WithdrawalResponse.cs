namespace AgroTemp.Domain.DTO.Payment;

public class WithdrawalResponse
{
    public Guid Id { get; set; }
    public string? PayoutId { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public string? ApprovalState { get; set; }
    public string? BankName { get; set; }
    public string? AccountHolderName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? ReferenceCode { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
}
