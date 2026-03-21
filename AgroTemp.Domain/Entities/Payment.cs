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
    public class Payment
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
        [Column("order_code")]
        public long OrderCode { get; set; } // PayOS orderCode

        [Required]
        [Column("amount",TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("paymnetLink_id")]
        public string PaymentLinkId { get; set; }
        [Column("checkout_id")]
        public string CheckoutUrl { get; set; }
        [Column("status")]
        public string Status { get; set; } // PENDING / PAID / CANCELLED
        [Column("description")]
        public string Description { get; set; }
        [Column("create_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }
    }
}
