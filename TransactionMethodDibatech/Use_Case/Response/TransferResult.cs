namespace TransactionMethodDibatech.Use_Case.Response
{
    public class TransferResult
    {
        public bool Succeeded { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public Guid? TransferId { get; set; }

        public static TransferResult Success(Guid key)
          => new() { Succeeded = true, TransferId = key };

        public static TransferResult Duplicate(Guid key)
            => new() { Succeeded = false, ErrorCode = "Duplicate", Message = "Transfer already completed", TransferId = key };

        public static TransferResult InsufficientFunds
            => new() { Succeeded = false, ErrorCode = "InsufficientFunds", Message = "Not enough balance" };

        public static TransferResult ConcurrencyConflict
            => new() { Succeeded = false, ErrorCode = "ConcurrencyConflict", Message = "Account was modified concurrently" };

        public static TransferResult InvalidOperation(string msg)
            => new() { Succeeded = false, ErrorCode = "InvalidOperation", Message = msg };
    }
}

