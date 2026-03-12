using AgroTemp.Domain.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public string? VnPayTxnRef { get; set; }
        public string? VnPayResponseCode { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
