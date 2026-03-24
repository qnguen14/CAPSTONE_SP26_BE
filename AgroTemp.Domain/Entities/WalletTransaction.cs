using AgroTemp.Domain.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Entities
{
    public enum TransactionType
    {
        DEPOSIT = 1,
        WITHDRAW = 2,
        JOB_PAYMENT = 3,
        REFUND = 4
    }

    public class WalletTransaction
    {
        [Key]
        [Required]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Wallet))]
        [Column("wallet_id")]
        public Guid WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }

        [Required]
        [ForeignKey(nameof (JobDetail))]
        [Column("job_detaid_id")]
        public Guid? JobDetailId { get; set; }
        public virtual JobDetail JobDetail { get; set; }

        [Required]
        [Column("type")]
        public TransactionType Type { get; set; }

        [Column("amount",TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("balance_after",TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; }

        [Column("reference_code")]
        public string ReferenceCode { get; set; }

        [Column("desription")]
        public string Description { get; set; }

        [Column("create_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
