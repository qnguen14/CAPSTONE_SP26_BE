using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
        
    }
}
