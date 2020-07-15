namespace AlterBankApi.Application.Commands
{
    using MediatR;
    using AlterBankApi.Application.Responses;

    public sealed class OpenAccountCommand : IRequest<OpenAccountResponse>
    {
        public string AccountNum { get; }
        public decimal Balance { get; }

        public OpenAccountCommand(string accountNum, decimal balance)
        {
            AccountNum = accountNum;
            Balance = balance;
        }
    }
}
