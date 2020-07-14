namespace AlterBankApi.Application.Responses
{
    public sealed class FundTransferResponse
    {
        public bool IsSuccess { get; }
        public string AccountNumDebit { get; }
        public decimal BalanceDebit { get; }
        public string AccountNumCredit { get; }
        public decimal BalanceCredit { get; }

        public FundTransferResponse(string accountNumCredit, decimal balanceCredit, string accountNumDebit, decimal balanceDebit, bool isSuccess)
        {
            AccountNumDebit = accountNumDebit;
            AccountNumCredit = accountNumCredit;
            BalanceDebit = balanceDebit;
            BalanceCredit = balanceCredit;
            IsSuccess = isSuccess;
        }
    }
}
