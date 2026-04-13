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
        Task RefundLockedAmountForJobPostAsync(Guid farmerUserId, Guid jobPostId, decimal amount);
        Task ApplyJobSettlementAsync(JobDetail jobDetail, decimal workerPaymentAmount, decimal refundAmount);

        /// <summary>
        /// Releases escrow from the farmer's locked balance and transfers the worker payment amount
        /// to the worker's wallet.
        /// - For Daily jobs: releases escrow equal to the day's JobPrice on every approved report.
        /// - For PerJob jobs: releases escrow only when this is the last job detail (last work day).
        /// </summary>
        Task ReleaseEscrowAndPayWorkerAsync(JobDetail jobDetail, decimal workerPaymentAmount, decimal refundAmount, bool isLastDetail);
    }
}