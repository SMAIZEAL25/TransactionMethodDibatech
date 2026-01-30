using AssignmentTransferMethod.DAL;
using AssignmentTransferMethod.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace AssignmentTransferMethod.Use_Case.Interface
{
    public class ERTransferRespository : ITransferRepository
    {
        public readonly TransferMethodDB _transferMethodDB;

        public ERTransferRespository(TransferMethodDB transferMethodDB)
        {
            _transferMethodDB = transferMethodDB;
        }
        public Task AddTransferAsync(Transfer transfer, CancellationToken ct = default)
        {
             _transferMethodDB.Transfers.AddAsync(transfer, ct);
            return Task.CompletedTask;
        }

        public async Task<Account?> GetAccountForUpdateAsync(Guid accountId, CancellationToken ct = default)
        {
            var transferAccount = await _transferMethodDB.Accounts.FromSqlInterpolated
                 ($"SELECT * FROM ACCOUNT WITH (UPDLOCK, ROWLOCK) WHERE ID = {accountId}").SingleOrDefaultAsync(ct);
            return transferAccount;
        }

        public Task SaveChangeAsync(CancellationToken ct = default)
        {
            return _transferMethodDB.SaveChangesAsync(ct);
        }

        public Task<bool> TransferExistsAsync(Guid idempotencyKey, CancellationToken ct = default)
        {
            var checkTransfer = _transferMethodDB.Transfers.AnyAsync(t => t.IdempotencyKey == idempotencyKey, ct);
            return checkTransfer;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct)
        {
            return await _transferMethodDB.Database.BeginTransactionAsync(ct);
        }
    }
}
