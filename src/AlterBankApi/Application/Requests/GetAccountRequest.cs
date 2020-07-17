namespace AlterBankApi.Application.Requests
{
    using MediatR;
    using AlterBankApi.Application.Model;
    using AlterBankApi.Application.Responses;

    /// <summary>
    /// The request to get <c>Account</c> by its number
    /// </summary>
    public sealed class GetAccountRequestById : IRequest<ExecutionResult<Account>>
    {
        /// <summary>
        /// The account number
        /// </summary>
        public string AccountNum { get; }

        /// <summary>
        /// Constructs the <c>GetAccountRequestById</c> instance
        /// </summary>
        /// <param name="accountNum">Account number to find</param>
        public GetAccountRequestById(string accountNum)
        {
            AccountNum = accountNum;
        }
    }
}
