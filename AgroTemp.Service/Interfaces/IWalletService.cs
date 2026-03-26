using AgroTemp.Domain.Entities;

namespace AgroTemp.Service.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> GetOrCreateWalletAsync(Guid userId);
        Task<Wallet?> GetByIdAsync(Guid walletId);
        Task<Wallet?> GetByUserIdAsync(Guid userId);
        Task<ICollection<Wallet>> GetAllAsync();
        Task LockAmountForJobPostAsync(Guid farmerUserId, Guid jobPostId, decimal amount);
        Task ApplyJobSettlementAsync(JobDetail jobDetail, decimal workerPaymentAmount, decimal refundAmount);
    }
}