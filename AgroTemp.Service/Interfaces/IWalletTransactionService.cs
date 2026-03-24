using AgroTemp.Domain.Entities;

namespace AgroTemp.Service.Interfaces
{
    public interface IWalletTransactionService
    {
        Task ApplyJobSettlementAsync(JobDetail jobDetail, decimal workerPaymentAmount, decimal refundAmount);
    }
}
