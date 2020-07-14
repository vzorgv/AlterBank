namespace AlterBankApi.Application.Responses
{
    using AlterBankApi.Application.DataModel;

    public sealed class GetAccountResponse
    {
        public string AccountNum { get; }
        public string CurrencyIsoCode { get; }
        public decimal Balance { get; }

        public GetAccountResponse(string accountNum, string currencyIsoCode, decimal balance)
        {
            AccountNum = accountNum;
            CurrencyIsoCode = currencyIsoCode;
            Balance = balance;
        }

        public GetAccountResponse(Account account)
            : this(account.AccountNum, account.CurrencyIsoCode, account.Balance)
        {
        }
    }
}
