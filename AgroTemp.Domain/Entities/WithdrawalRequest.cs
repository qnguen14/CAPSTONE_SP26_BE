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
    public class WithdrawalRequest
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
        [Column("amount",TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column("bank_account_number")]
        public string BankAccountNumber { get; set; }

        [Required]
        [Column("bank_name")]
        public string BankName { get; set; }

        [Column("account_holder_name")]
        public string AccountHolderName { get; set; }

        [Column("status")]
        public string Status { get; set; } // PENDING / APPROVED / REJECTED / PAID

        [Column("note")]
        public string Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("processed_at")]
        public DateTime? ProcessedAt { get; set; }
    }
}
