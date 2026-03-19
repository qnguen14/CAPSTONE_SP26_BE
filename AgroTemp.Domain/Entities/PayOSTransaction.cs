using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("PayOS_Transaction")]
public class PayOSTransaction
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Order))]
    [Column("order_id")]
    public Guid OrderId { get; set; }
    public virtual PayOSOrder Order { get; set; }

    [Column("reference")]
    [StringLength(128)]
    public string? Reference { get; set; }

    [Required]
    [Column("amount")]
    public long Amount { get; set; }

    [Column("account_number")]
    [StringLength(64)]
    public string? AccountNumber { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("transaction_datetime")]
    public DateTime TransactionDateTime { get; set; }

    [Column("virtual_account_name")]
    [StringLength(256)]
    public string? VirtualAccountName { get; set; }

    [Column("virtual_account_number")]
    [StringLength(128)]
    public string? VirtualAccountNumber { get; set; }

    [Column("counter_account_bank_id")]
    [StringLength(64)]
    public string? CounterAccountBankId { get; set; }

    [Column("counter_account_bank_name")]
    [StringLength(256)]
    public string? CounterAccountBankName { get; set; }

    [Column("counter_account_name")]
    [StringLength(256)]
    public string? CounterAccountName { get; set; }

    [Column("counter_account_number")]
    [StringLength(128)]
    public string? CounterAccountNumber { get; set; }
}
