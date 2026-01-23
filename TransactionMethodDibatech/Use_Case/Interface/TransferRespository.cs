using TransactionMethodDibatech.Enitites;

namespace TransactionMethodDibatech.Use_Case.Interface
{
    public class TransferRespository : ITransferRepository
    {
        public Task AddTransferAsync(Transfer transfer, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Account?> GetAccountForUpdateAsync(Guid accountId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangeAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TransferExistsAsync(Guid idempotencyKey, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
