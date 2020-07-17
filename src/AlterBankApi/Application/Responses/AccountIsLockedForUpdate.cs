namespace AlterBankApi.Application.Responses
{
    /// <summary>
    /// The locked account description
    /// </summary>
    public sealed class AccountIsLockedForUpdate : IExecutionResultDescription
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message => "One of the accounts you are being update is modified by another transaction";
    }
}
