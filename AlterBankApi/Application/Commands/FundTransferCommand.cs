namespace AlterBankApi.Application.Commands
{
    using MediatR;
    using AlterBankApi.Application.Responses;

    public sealed class FundTransferCommand : IRequest<FundTransferResponse>
    {
        public string AccountNumDebit { get; }
        public string AccountNumCredit { get; }
        public decimal Amount { get; }

        public FundTransferCommand(string accountNumCredit, string accountNumDebit, decimal amount)
        {
            AccountNumDebit = accountNumDebit;
            AccountNumCredit = accountNumCredit;
            Amount = amount;
        }
    }
}
