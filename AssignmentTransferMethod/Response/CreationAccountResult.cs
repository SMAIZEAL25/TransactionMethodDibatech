namespace AssignmentTransferMethod.Response
{
    public class CreationAccountResult
    {
        public bool Succeeded { get; set; }

        public string ErrorCode { get; set; }
        public string Message { get; set; }

        public Guid? AccountId { get; set; }

        public static CreationAccountResult Success(Guid accountId) => new() { Succeeded = true, AccountId = accountId };

        public static CreationAccountResult Duplicate(Guid? accountId = null) => new()
        { Succeeded = false, ErrorCode = "Duplicate", Message = "Account already created", AccountId = accountId };

        public static CreationAccountResult InvalidOperation(string msg) => new() { Succeeded = false, ErrorCode = "Invalid Operation", Message = msg };
    }
}

