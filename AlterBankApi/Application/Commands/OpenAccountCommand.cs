namespace AlterBankApi.Application.Commands
{
    using MediatR;
    using AlterBankApi.Application.Responses;

    public sealed class OpenAccountCommand : IRequest<OpenAccountResponse>
    {
        public string AccountNum { get; }
        public string CurrencyIsoCode { get; }
        public decimal Balance { get; }

        public OpenAccountCommand(string accountNum, string currencyIsoCode, decimal balance)
        {
            AccountNum = accountNum;
            CurrencyIsoCode = currencyIsoCode;
            Balance = balance;
        }
    }
}
