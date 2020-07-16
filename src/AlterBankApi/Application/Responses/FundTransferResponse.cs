namespace AlterBankApi.Application.Responses
{
    /// <summary>
    /// The result of transfer command
    /// </summary>
    public sealed class FundTransferResponse
    {
        /// <summary>
        /// True if command succeded otherwise false
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Withdrew account number
        /// </summary>
        public string AccountNumDebit { get; }
        
        /// <summary>
        /// Balance
        /// </summary>
        public decimal BalanceDebit { get; }

        /// <summary>
        /// Deposited account number
        /// </summary>
        public string AccountNumCredit { get; }

        /// <summary>
        /// Balance
        /// </summary>
        public decimal BalanceCredit { get; }

        /// <summary>
        /// Constructs the <c>FundTransferResponse</c> instance
        /// </summary>
        /// <param name="accountNumCredit">Credit account numebr</param>
        /// <param name="balanceCredit">Balance</param>
        /// <param name="accountNumDebit">Debit account numebr</param>
        /// <param name="balanceDebit">Balance</param>
        /// <param name="isSuccess">Transfer result</param>
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
