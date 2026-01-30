namespace AssignmentTransferMethod.Entities
{
    public class AccountCreation
    {
        public Guid IdempotencyKey { get; set; }
        public string Name { get; set; }
        public Guid AccountId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Account Account { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
