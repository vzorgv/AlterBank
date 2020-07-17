namespace AlterBankApi.Application.Responses
{
    /// <summary>
    /// Represnt the not valid state of <c>Account</c> entity description
    /// </summary>
    public sealed class AccountNotInValidStateDescription : IExecutionResultDescription
    {
        /// <summary>
        /// The result message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Constructs <c>NotValidAccountStateDescription</c> instance
        /// </summary>
        /// <param name="message">Error message</param>
        public AccountNotInValidStateDescription(string message)
        {
            Message = message;
        }
    }
}
