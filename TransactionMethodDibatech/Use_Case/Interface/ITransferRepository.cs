using TransactionMethodDibatech.Enitites;

namespace TransactionMethodDibatech.Use_Case.Interface
{
    public interface ITransferRepository
    {
        Task<Account?> GetAccountForUpdateAsync(Guid accountId, CancellationToken ct = default);
        Task<bool> TransferExistsAsync(Guid idempotencyKey, CancellationToken ct = default);
        Task AddTransferAsync(Transfer transfer, CancellationToken ct = default);
        Task SaveChangeAsync (CancellationToken ct = default);
    }
}
