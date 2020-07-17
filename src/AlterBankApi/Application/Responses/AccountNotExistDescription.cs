namespace AlterBankApi.Application.Responses
{
    /// <summary>
    /// Not found description
    /// </summary>
    public sealed class AccountNotExistDescription : IExecutionResultDescription
    {
        /// <summary>
        /// The result message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Constructs <c>AccountNotFoundDescription</c> instance
        /// </summary>
        /// <param name="message">Error message</param>
        public AccountNotExistDescription(string message)
        {
            Message = message;
        }
    }
}
