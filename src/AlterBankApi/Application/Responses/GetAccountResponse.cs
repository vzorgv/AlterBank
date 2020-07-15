namespace AlterBankApi.Application.Responses
{
    using AlterBankApi.Application.DataModel;

    public sealed class GetAccountResponse
    {
        public string AccountNum { get; }
        public decimal Balance { get; }

        public GetAccountResponse(string accountNum, decimal balance)
        {
            AccountNum = accountNum;
            Balance = balance;
        }

        public GetAccountResponse(Account account)
            : this(account.AccountNum, account.Balance)
        {
        }
    }
}
