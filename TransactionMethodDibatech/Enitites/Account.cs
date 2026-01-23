namespace TransactionMethodDibatech.Enitites
{
    public class Account
    {
        public Guid Id { get; set; }

        public decimal Balance { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<Tranfer> OutgoingTranfers { get; set; }
        public ICollection<Tranfer> IncomingTranfers { get; set; }
    }
}
