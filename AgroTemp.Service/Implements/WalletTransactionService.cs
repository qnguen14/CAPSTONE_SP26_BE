using AgroTemp.Domain.Context;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AgroTemp.Service.Implements
{
    public class WalletTransactionService : BaseService<WalletTransaction>, IWalletTransactionService
    {
        public WalletTransactionService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task CreateAsync(Wallet wallet, Guid? jobDetailId, TransactionType type, decimal amount, decimal balanceAfter, string referenceCode, string description)
        {
            await _unitOfWork.GetRepository<WalletTransaction>().InsertAsync(new WalletTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                Wallet = wallet,
                JobDetailId = jobDetailId,
                Type = type,
                Amount = amount,
                BalanceAfter = balanceAfter,
                ReferenceCode = referenceCode,
                Description = description,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<WalletTransaction?> GetByIdAsync(Guid transactionId)
        {
            return await _unitOfWork.GetRepository<WalletTransaction>()
                .FirstOrDefaultAsync(predicate: x => x.Id == transactionId);
        }

        public async Task<ICollection<WalletTransaction>> GetAllAsync()
        {
            return await _unitOfWork.GetRepository<WalletTransaction>()
                .GetListAsync();
        }

        public async Task<ICollection<WalletTransaction>> GetByWalletIdAsync(Guid walletId)
        {
            return await _unitOfWork.GetRepository<WalletTransaction>()
                .GetListAsync(
                    predicate: x => x.WalletId == walletId,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt));
        }
    }
}
