using AssignmentTransferMethod.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;

namespace AssignmentTransferMethod.Interface
{
    public interface IAccountRepository
    {
        Task<bool> AccountCreationExistAsync(Guid idempotencyKey, CancellationToken ct);

        Task<AccountCreation?> GetAccountCreationAsync(Guid idempotencyKey, CancellationToken ct);

        Task AddAccountAsync(Account account, CancellationToken ct);

        Task AddAccountCreationAsync(AccountCreation accountCreation, CancellationToken ct = default);

        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level, CancellationToken ct);

    }
}
