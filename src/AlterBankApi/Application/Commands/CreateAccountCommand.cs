namespace AlterBankApi.Application.Commands
{
    using MediatR;
    using AlterBankApi.Application.Model;

    /// <summary>
    /// Comand to create Account
    /// </summary>
    public sealed class CreateAccountCommand : IRequest<Account>
    {
        /// <summary>
        /// Account to create
        /// </summary>
        public Account Account { get; }

        /// <summary>
        /// Constructs <c>CreateAccountCommand</c> instance
        /// </summary>
        /// <param name="account"></param>
        public CreateAccountCommand(Account account)
        {
            Account = account;
        }
    }
}
