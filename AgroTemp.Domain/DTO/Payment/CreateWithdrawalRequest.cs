using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Payment;

public class CreateWithdrawalRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public string ToBin { get; set; } = string.Empty;

    [Required]
    public string ToAccountNumber { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<string>? Category { get; set; }

    public string? BankName { get; set; }

    public string? AccountHolderName { get; set; }
}
