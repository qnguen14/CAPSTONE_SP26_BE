using AgroTemp.Domain.Context;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AgroTemp.Service.Implements
{
    public class WalletService : BaseService<Wallet>, IWalletService
    {
        private readonly IWalletTransactionService _walletTransactionService;

        public WalletService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper,
            IWalletTransactionService walletTransactionService) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _walletTransactionService = walletTransactionService;
        }

        public async Task ApplyJobSettlementAsync(JobDetail jobDetail, decimal workerPaymentAmount, decimal refundAmount)
        {
            if (workerPaymentAmount < 0)
            {
                throw new Exception("Worker payment amount cannot be negative.");
            }

            if (refundAmount < 0)
            {
                throw new Exception("Refund amount cannot be negative.");
            }

            if (workerPaymentAmount == 0 && refundAmount == 0)
            {
                return;
            }

            var worker = await _unitOfWork.GetRepository<Worker>()
                .FirstOrDefaultAsync(predicate: w => w.Id == jobDetail.WorkerId);
            if (worker == null)
            {
                throw new Exception("Worker not found for job settlement.");
            }

            var jobPost = await _unitOfWork.GetRepository<JobPost>()
                .FirstOrDefaultAsync(predicate: jp => jp.Id == jobDetail.JobPostId);
            if (jobPost == null)
            {
                throw new Exception("Job post not found for job settlement.");
            }

            var farmer = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(predicate: f => f.Id == jobPost.FarmerId);
            if (farmer == null)
            {
                throw new Exception("Farmer not found for job settlement.");
            }

            var workerWallet = await GetOrCreateWalletAsync(worker.UserId);
            var farmerWallet = await GetOrCreateWalletAsync(farmer.UserId);

            try
            {
                if (workerPaymentAmount > 0)
                {
                    workerWallet.Balance += workerPaymentAmount;
                    workerWallet.UpdateAt = DateTime.UtcNow;

                    await _walletTransactionService.CreateAsync(
                        wallet: workerWallet,
                        jobDetailId: jobDetail.Id,
                        type: TransactionType.JOB_PAYMENT,
                        amount: workerPaymentAmount,
                        balanceAfter: workerWallet.Balance,
                        referenceCode: $"JOB-{jobDetail.Id:N}-PAY",
                        description: $"Worker payment for job detail {jobDetail.Id}");
                }

                if (refundAmount > 0)
                {
                    farmerWallet.Balance += refundAmount;
                    farmerWallet.UpdateAt = DateTime.UtcNow;

                    await _walletTransactionService.CreateAsync(
                        wallet: farmerWallet,
                        jobDetailId: jobDetail.Id,
                        type: TransactionType.REFUND,
                        amount: refundAmount,
                        balanceAfter: farmerWallet.Balance,
                        referenceCode: $"JOB-{jobDetail.Id:N}-REFUND",
                        description: $"Refund for job detail {jobDetail.Id}");
                }
            }
            catch (Exception ex) when (IsSchemaMismatch(ex))
            {
                if (workerPaymentAmount > 0)
                {
                    await CreditLegacyWalletAsync(
                        userId: worker.UserId,
                        amount: workerPaymentAmount,
                        type: TransactionType.JOB_PAYMENT,
                        referenceId: $"JOB-{jobDetail.Id:N}-PAY",
                        description: $"Worker payment for job detail {jobDetail.Id}");
                }

                if (refundAmount > 0)
                {
                    await CreditLegacyWalletAsync(
                        userId: farmer.UserId,
                        amount: refundAmount,
                        type: TransactionType.REFUND,
                        referenceId: $"JOB-{jobDetail.Id:N}-REFUND",
                        description: $"Refund for job detail {jobDetail.Id}");
                }
            }
        }

        public async Task<Wallet> GetOrCreateWalletAsync(Guid userId)
        {
            var wallet = await _unitOfWork.GetRepository<Wallet>()
                .FirstOrDefaultAsync(predicate: w => w.UserId == userId);

            if (wallet != null)
            {
                return wallet;
            }

            wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 0,
                LockedBalance = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Wallet>().InsertAsync(wallet);
            return wallet;
        }

        public async Task LockAmountForJobPostAsync(Guid farmerUserId, Guid jobPostId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount to lock must be greater than 0.", nameof(amount));
            }

            var wallet = await GetOrCreateWalletAsync(farmerUserId);

            if (wallet.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient wallet balance to create job post.");
            }

            wallet.Balance -= amount;
            wallet.LockedBalance += amount;
            wallet.UpdateAt = DateTime.UtcNow;

            await _walletTransactionService.CreateAsync(
                wallet: wallet,
                jobDetailId: null,
                type: TransactionType.JOB_LOCK,
                amount: amount,
                balanceAfter: wallet.Balance,
                referenceCode: $"JOB-{jobPostId:N}-LOCK",
                description: $"Lock funds for job post {jobPostId}");
        }

        private static bool IsSchemaMismatch(Exception ex)
        {
            if (ex is PostgresException pg && (pg.SqlState == "42703" || pg.SqlState == "42P01"))
            {
                return true;
            }

            return ex.InnerException != null && IsSchemaMismatch(ex.InnerException);
        }

        private async Task CreditLegacyWalletAsync(
            Guid userId,
            decimal amount,
            TransactionType type,
            string referenceId,
            string description)
        {
            var conn = _unitOfWork.Context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
            }

            var walletId = Guid.Empty;
            decimal balance = 0;

            await using (var getCmd = conn.CreateCommand())
            {
                getCmd.CommandText = @"
                SELECT ""Id"", ""Balance""
                FROM ""AgroTempV1"".""Wallets""
                WHERE ""UserId"" = @userId
                LIMIT 1;";

                var userParam = getCmd.CreateParameter();
                userParam.ParameterName = "@userId";
                userParam.Value = userId;
                getCmd.Parameters.Add(userParam);

                await using var reader = await getCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    walletId = reader.GetGuid(0);
                    balance = reader.GetDecimal(1);
                }
            }

            if (walletId == Guid.Empty)
            {
                walletId = Guid.NewGuid();
                await using var insertWalletCmd = conn.CreateCommand();
                insertWalletCmd.CommandText = @"
INSERT INTO ""AgroTempV1"".""Wallets"" (""Id"", ""UserId"", ""Balance"", ""CreatedAt"")
VALUES (@id, @userId, 0, @createdAt);";

                var idParam = insertWalletCmd.CreateParameter();
                idParam.ParameterName = "@id";
                idParam.Value = walletId;
                insertWalletCmd.Parameters.Add(idParam);

                var userParam = insertWalletCmd.CreateParameter();
                userParam.ParameterName = "@userId";
                userParam.Value = userId;
                insertWalletCmd.Parameters.Add(userParam);

                var createdAtParam = insertWalletCmd.CreateParameter();
                createdAtParam.ParameterName = "@createdAt";
                createdAtParam.Value = DateTime.UtcNow;
                insertWalletCmd.Parameters.Add(createdAtParam);

                await insertWalletCmd.ExecuteNonQueryAsync();
            }

            var updatedBalance = balance + amount;

            await using (var updateWalletCmd = conn.CreateCommand())
            {
                updateWalletCmd.CommandText = @"
UPDATE ""AgroTempV1"".""Wallets""
SET ""Balance"" = @balance
WHERE ""Id"" = @walletId;";

                var balParam = updateWalletCmd.CreateParameter();
                balParam.ParameterName = "@balance";
                balParam.Value = updatedBalance;
                updateWalletCmd.Parameters.Add(balParam);

                var walletParam = updateWalletCmd.CreateParameter();
                walletParam.ParameterName = "@walletId";
                walletParam.Value = walletId;
                updateWalletCmd.Parameters.Add(walletParam);

                await updateWalletCmd.ExecuteNonQueryAsync();
            }

            await using (var insertTxnCmd = conn.CreateCommand())
            {
                insertTxnCmd.CommandText = @"
INSERT INTO ""AgroTempV1"".""WalletTransactions"" (
    ""Id"", ""WalletId"", ""Amount"", ""Type"", ""ReferenceId"", ""Description"", ""CreatedAt"")
VALUES (
    @id, @walletId, @amount, @type, @referenceId, @description, @createdAt);";

                var idParam = insertTxnCmd.CreateParameter();
                idParam.ParameterName = "@id";
                idParam.Value = Guid.NewGuid();
                insertTxnCmd.Parameters.Add(idParam);

                var walletParam = insertTxnCmd.CreateParameter();
                walletParam.ParameterName = "@walletId";
                walletParam.Value = walletId;
                insertTxnCmd.Parameters.Add(walletParam);

                var amountParam = insertTxnCmd.CreateParameter();
                amountParam.ParameterName = "@amount";
                amountParam.Value = amount;
                insertTxnCmd.Parameters.Add(amountParam);

                var typeParam = insertTxnCmd.CreateParameter();
                typeParam.ParameterName = "@type";
                typeParam.Value = (int)type;
                insertTxnCmd.Parameters.Add(typeParam);

                var referenceParam = insertTxnCmd.CreateParameter();
                referenceParam.ParameterName = "@referenceId";
                referenceParam.Value = referenceId;
                insertTxnCmd.Parameters.Add(referenceParam);

                var descParam = insertTxnCmd.CreateParameter();
                descParam.ParameterName = "@description";
                descParam.Value = description;
                insertTxnCmd.Parameters.Add(descParam);

                var createdAtParam = insertTxnCmd.CreateParameter();
                createdAtParam.ParameterName = "@createdAt";
                createdAtParam.Value = DateTime.UtcNow;
                insertTxnCmd.Parameters.Add(createdAtParam);

                await insertTxnCmd.ExecuteNonQueryAsync();
            }
        }
    }
}