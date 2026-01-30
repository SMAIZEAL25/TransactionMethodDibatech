using AssignmentTransferMethod.Entities;
using AssignmentTransferMethod.Interface;
using AssignmentTransferMethod.Response;
using AssignmentTransferMethod.Use_Case.Interface;
using Microsoft.EntityFrameworkCore;


namespace AssignmentTransferMethod.Servies
{
    public class TransferServices
    {
        private readonly ITransferRepository _transferRepository;
        private readonly IAccountRepository _accountRepository;

        public TransferServices(ITransferRepository transferRepository, IAccountRepository accountRepository)
        {
            _transferRepository = transferRepository;
            _accountRepository = accountRepository;
        }
        public async Task<CreationAccountResult> CreateAccountAsync(decimal initialBalance,string idempotencyKeyStr,CancellationToken ct = default)
        {
            if (initialBalance < 0)
                return CreationAccountResult.InvalidOperation("Initial balance cannot be negative");

            if (!Guid.TryParse(idempotencyKeyStr, out var idempotencyKey))
                return CreationAccountResult.InvalidOperation("Invalid idempotency key format");

            // ────────────────────────────────────────────────────────────────
            //  1. Check if this exact creation already happened
            // ────────────────────────────────────────────────────────────────
            var existing = await _accountRepository.GetAccountCreationAsync(idempotencyKey, ct);
            if (existing != null)
            {
                return CreationAccountResult.Duplicate(existing.AccountId);
            }

            // ────────────────────────────────────────────────────────────────
            //  2. Start explicit transaction
            // ────────────────────────────────────────────────────────────────
            await using var transaction = await _accountRepository.TransferMethodDB.BeginTransactionAsync(
                IsolationLevel.ReadCommitted, ct);

            try
            {
                var accountId = Guid.NewGuid();
                var account = new Account
                {
                    Id = accountId,
                    Balance = initialBalance
                };

                var creation = new AccountCreation
                {
                    IdempotencyKey = idempotencyKey,
                    AccountId = accountId,
                    InitialBalance = initialBalance
                };

                await _accountRepository.AddAccountAsync(account, ct);
                await _accountRepository.AddAccountCreationAsync(creation, ct);

                await _accountRepository.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                return CreationAccountResult.Success(accountId);
            }
            catch (Exception ex) when (IsUniqueKeyViolation(ex))
            {
                // Race: another request inserted first → fetch existing
                await transaction.RollbackAsync(ct);

                existing = await _accountRepository.GetAccountCreationAsync(idempotencyKey, ct);
                return existing != null
                    ? CreationAccountResult.Duplicate(existing.AccountId)
                    : throw; // Rare, rethrow
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        // Reuse IsUniqueKeyViolation from previous
        private static bool IsUniqueKeyViolation(Exception ex)
        {
            if (ex is DbUpdateException dbEx &&
                dbEx.InnerException != null)
            {
                return dbEx.InnerException.Message.Contains("UNIQUE");
            }

            return false;
        }

        public async Task<TransferResult> TransferAsync(Guid fromAccountId,Guid toAccountId,decimal amount,Guid idempotencyKey,CancellationToken ct = default)
        {
            using var transaction = await _transferRepository.BeginTransactionAsync(ct);

            // Check idempotency
            var existingTransfer = await _transferRepository.GetAccountForUpdateAsync(idempotencyKey, ct);

            if (existingTransfer != null)
            {
                return TransferResult.Duplicate(existingTransfer.Id);
            }

            // Get accounts
            var fromAccount = await _transferRepository.GetAccountForUpdateAsync(fromAccountId, ct);
            var toAccount = await _transferRepository.GetAccountForUpdateAsync(toAccountId, ct);

            if (fromAccount == null || toAccount == null)
            {
                return TransferResult.AccountNotFound;
            }

            if (amount <= 0)
            {
                return TransferResult.InvalidOperation("Transfer amount must be greater than zero.");
            }

            if (fromAccount.Balance < amount)
            {
                return TransferResult.InsufficientFunds;
            }

            try
            {
                // Perform transfer
                fromAccount.Balance -= amount;
                toAccount.Balance += amount;

                var transfer = new Entities.Transfer
                {
                    id = Guid.NewGuid(),
                    FromAccountId = fromAccountId,
                    ToAccountId = toAccountId,
                    Amount = amount,
                    IdempotencyKey = idempotencyKey,
                    CreatedAt = DateTime.UtcNow
                };

                await _transferRepository.AddTransferAsync(transfer, ct);
                await _transferRepository.SaveChangeAsync(ct);
                await transaction.CommitAsync(ct);

                return TransferResult.Success(transfer.id);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(ct);
                return TransferResult.ConcurrencyConflict;
            }
        }

    }
}
