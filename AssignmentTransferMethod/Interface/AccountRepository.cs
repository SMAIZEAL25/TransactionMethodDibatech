using AssignmentTransferMethod.DAL;
using AssignmentTransferMethod.Entities;
using AssignmentTransferMethod.Use_Case.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Transactions;

namespace AssignmentTransferMethod.Interface
{
    public class EfAccountRepository : ERTransferRespository, IAccountRepository
    {
        public EfAccountRepository(TransferMethodDB transferMethod) : base(transferMethod) { }
        public Task<bool> AccountCreationExistAsync(Guid idempotencyKey, CancellationToken ct)
        {
            var cheack = _transferMethodDB.Set<AccountCreation>().AnyAsync(ac => ac.IdempotencyKey == idempotencyKey, ct);
            return cheack;
        }

        public Task AddAccountAsync(Account account, CancellationToken ct)
        {
            _transferMethodDB.Set<Account>().AddAsync(account, ct);
            return Task.CompletedTask;
        }

        public Task AddAccountCreationAsync(AccountCreation accountCreation, CancellationToken ct = default)
        {
            _transferMethodDB.Set<AccountCreation>().AddAsync(accountCreation, ct);
            return Task.CompletedTask;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level, CancellationToken ct)
        {
            return _transferMethodDB.Database.BeginTransactionAsync(level, ct);
        }

        public async Task<AccountCreation?> GetAccountCreationAsync(Guid idempotencyKey, CancellationToken ct)
        {
            var check = await _transferMethodDB.Set<AccountCreation>().SingleOrDefaultAsync(c => c.IdempotencyKey == idempotencyKey, ct);
            return check;
        }

        Task<int> IAccountRepository.SaveChangesAsync(CancellationToken ct)
        {
            var save = _transferMethodDB.SaveChangesAsync(ct);
            return save;
        }
    }
}
