namespace AlterBankApi.Application.DataModel
{
    public sealed class Account
    {
        public string AccountNum { get; set; }
        public decimal Balance { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
