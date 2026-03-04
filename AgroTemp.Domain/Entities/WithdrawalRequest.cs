using AgroTemp.Domain.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Entities
{
    public class WithdrawalRequest
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public decimal Amount { get; set; }

        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string AccountHolderName { get; set; }

        public WithdrawalStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
