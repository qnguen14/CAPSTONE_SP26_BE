using AgroTemp.Domain.Entities;

namespace AgroTemp.Service.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> GetOrCreateWalletAsync(Guid userId);
        Task LockAmountForJobPostAsync(Guid farmerUserId, Guid jobPostId, decimal amount);
        Task ApplyJobSettlementAsync(JobDetail jobDetail, decimal workerPaymentAmount, decimal refundAmount);
    }
}