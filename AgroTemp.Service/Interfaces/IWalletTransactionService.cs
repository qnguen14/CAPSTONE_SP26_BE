using AgroTemp.Domain.Entities;

namespace AgroTemp.Service.Interfaces
{
    public interface IWalletTransactionService
    {
        Task CreateAsync(Wallet wallet, Guid? jobDetailId, TransactionType type, decimal amount, decimal balanceAfter, string referenceCode, string description);
        Task<WalletTransaction?> GetByIdAsync(Guid transactionId);
        Task<ICollection<WalletTransaction>> GetAllAsync();
        Task<ICollection<WalletTransaction>> GetByWalletIdAsync(Guid walletId);
    }
}
