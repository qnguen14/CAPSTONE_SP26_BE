using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Metadata
{
    public enum FarmType
    {
        Livestock = 1,  
        Crop = 2        
    }

    public enum WalletTransactionType
    {
        Deposit = 1,
        JobHold = 2,
        JobPayment = 3,
        Refund = 4,
        Withdrawal = 5
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3
    }

    public enum WithdrawalStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
