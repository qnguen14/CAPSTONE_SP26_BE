using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Payment;

public enum BinBank
{
    Vietcombank = 970436,
    BIDV = 970418,
    VietinBank = 970415,
    Agribank = 970405,
    MB = 970422,
    ACB = 970416,
    Techcombank = 970407,
    Sacombank = 970403,
    VPBank = 970432,
    TPBank = 970423,
    VIB = 970441,
    HDBank = 970437,
    SHB = 970443,
    OCB = 970448,
    MSB = 970426,
    SeABank = 970440,
    Eximbank = 970431,
    NamABank = 970428,
    PVcomBank = 970412,
    LPBank = 970449,
    NCB = 970419,
    SCB = 970429,
    Saigonbank = 970400,
    BaoVietBank = 970438,
    KienlongBank = 970452,
    VietBank = 970454,
    CoOpBank = 970446,
    PublicBank = 970439,
    WooriBank = 970457,
    UOB = 970458
}

public class CreateWithdrawalRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [EnumDataType(typeof(BinBank), ErrorMessage = "ToBin must be a valid bank BIN from BinBank enum")]
    public BinBank ToBin { get; set; }

    [Required]
    public string ToAccountNumber { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<string> Category { get; set; } = new();

    public string? AccountHolderName { get; set; }
}
