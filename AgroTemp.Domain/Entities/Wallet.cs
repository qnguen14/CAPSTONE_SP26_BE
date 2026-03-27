using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Entities
{

    [Table("Wallet")]
    public class Wallet
    {
        [Key]
        [Required]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        [Column("user_id")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [Column("balance",TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Column("locked_balance",TypeName = "decimal(18,2)")]
        public decimal LockedBalance { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("create_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("update_at")]
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

        public virtual ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();

        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();

    }
}
