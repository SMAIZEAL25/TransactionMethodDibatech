using AssignmentTransferMethod.Response;
using AssignmentTransferMethod.Use_Case.Interface;
using Microsoft.EntityFrameworkCore;


namespace AssignmentTransferMethod.Servies
{
    public class TransferServices
    {
        private readonly ITransferRepository _transferRepository;

        public TransferServices(ITransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        // Main logic 

        //    public async Task<TransferResult> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, Guid idempotencyKey, CancellationToken ct = default)
        //    {
        //        // Check if transfer with the same idempotency key exists
        //        if (await _transferRepository.TransferExistsAsync(idempotencyKey, ct))
        //        {
        //            return TransferResult.Duplicate(existingTranfer.id);
        //        }
        //        // Get accounts for update
        //        var fromAccount = await _transferRepository.GetAccountForUpdateAsync(fromAccountId, ct);
        //        var toAccount = await _transferRepository.GetAccountForUpdateAsync(toAccountId, ct);
        //        if (fromAccount == null || toAccount == null)
        //        {
        //            return TransferResult.AccountNotFound;
        //        }
        //        if (fromAccount.Balance < amount)
        //        {
        //            return TransferResult.InsufficientFunds;
        //        }
        //        // Perform transfer
        //        fromAccount.Balance -= amount;
        //        toAccount.Balance += amount;
        //        // Create transfer record
        //        var transfer = new Entities.Transfer
        //        {
        //            id = Guid.NewGuid(),
        //            FromAccountId = fromAccountId,
        //            ToAccountId = toAccountId,
        //            Amount = amount,
        //            IdempotencyKey = idempotencyKey,
        //            CreatedAt = DateTime.UtcNow
        //        };
        //        await _transferRepository.AddTransferAsync(transfer, ct);
        //        await _transferRepository.SaveChangeAsync(ct);
        //        return TransferResult.Success;
        //    }
        //}
        public async Task<TransferResult> TransferAsync(
        Guid fromAccountId,
        Guid toAccountId,
        decimal amount,
        Guid idempotencyKey,
        CancellationToken ct = default)
        {
            using var transaction = await _transferRepository.BeginTransactionAsync(ct);

            // Check idempotency
            var existingTransfer = await _transferRepository
                .GetAccountForUpdateAsync(idempotencyKey, ct);

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
