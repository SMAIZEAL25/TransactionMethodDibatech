namespace AssignmentTransferMethod.Entities
{
    public class Account
    {
        public Guid Id { get; set; }

        public decimal Balance { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<Transfer> OutgoingTranfers { get; set; }
        public ICollection<Transfer> IncomingTranfers { get; set; }
    }
}

