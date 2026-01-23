namespace AssignmentTransferMethod.Response
{
    public class TransferResult
    {
        public bool Succeeded { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public Guid? TransferId { get; set; }

        public static TransferResult Success(Guid transferId)
            => new()
            {
                Succeeded = true,
                TransferId = transferId,
                Message = "Transfer completed successfully"
            };

        public static TransferResult Duplicate(Guid transferId)
            => new()
            {
                Succeeded = false,
                ErrorCode = "Duplicate",
                Message = "Transfer already completed",
                TransferId = transferId
            };

        public static TransferResult InsufficientFunds
            => new()
            {
                Succeeded = false,
                ErrorCode = "InsufficientFunds",
                Message = "Not enough balance"
            };

        public static TransferResult AccountNotFound
            => new()
            {
                Succeeded = false,
                ErrorCode = "AccountNotFound",
                Message = "One or both accounts were not found"
            };

        public static TransferResult ConcurrencyConflict
            => new()
            {
                Succeeded = false,
                ErrorCode = "ConcurrencyConflict",
                Message = "Account was modified concurrently"
            };

        public static TransferResult InvalidOperation(string msg)
            => new()
            {
                Succeeded = false,
                ErrorCode = "InvalidOperation",
                Message = msg
            };
    }
}

