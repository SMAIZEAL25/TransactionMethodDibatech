namespace AssignmentTransferMethod.DTO
{
    public class TransferRequest
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public Guid IdempotencyKey { get; set; }
    }
}
