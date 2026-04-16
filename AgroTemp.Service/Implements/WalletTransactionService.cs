using AgroTemp.Domain.Context;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Domain.Metadata;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PaginatedResponse<WalletTransaction>> GetByWalletIdAsync(Guid walletId, int page, int limit)
        {
            try
            {
                page = page < 1 ? 1 : page;
                limit = limit < 1 ? 10 : limit;
                var skip = (page - 1) * limit;
                var total = await _unitOfWork.GetRepository<WalletTransaction>()
                    .CountAsync(predicate: x => x.WalletId == walletId);

                var query = _unitOfWork.GetRepository<WalletTransaction>().CreateBaseQuery(
                    predicate: x => x.WalletId == walletId,
                    orderBy: q => q.OrderBy(x => x.CreatedAt),
                    include: q => q.Include(x => x.Wallet),
                    asNoTracking: true
                );
                
                var transactions = await query.Skip(skip).Take(limit).ToListAsync();
                return new PaginatedResponse<WalletTransaction>
                {
                    Data = transactions,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        Limit = limit,
                        Total = total,
                        TotalPages = (int)Math.Ceiling((double)total / limit)
                    }
                };
            } catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"Error in GetByWalletIdAsync: {ex.Message}");
                throw; // Rethrow the exception to be handled by the caller
            }
        }
    }
}
