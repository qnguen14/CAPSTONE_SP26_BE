using AgroTemp.Domain.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Entities
{
    public class WalletTransaction
    {
        public Guid Id { get; set; }

        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; }

        public decimal Amount { get; set; } 

        public WalletTransactionType Type { get; set; }

        public string? ReferenceId { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
