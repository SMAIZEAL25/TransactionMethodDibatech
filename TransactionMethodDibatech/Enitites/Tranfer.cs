namespace TransactionMethodDibatech.Enitites
{
    public class Transfer
    {
        public Guid IdempotencyKey { get; set; }

        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Completed";

        public Account FromAccount { get; set; }
        public Account ToAccount { get; set; }
    }
}
