namespace AlterBankApi.Application.Handlers
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using AlterBankApi.Application.Commands;
    using AlterBankApi.Application.Model;
    using AlterBankApi.Application.Responses;
    using AlterBankApi.Infrastructure;
    using AlterBankApi.Infrastructure.Repositories;
    using System.Data.SqlClient;

    /// <summary>
    /// Handles commands which modify account state
    /// </summary>
    public class AccountCommandHandler :
        IRequestHandler<CreateAccountCommand, ExecutionResult<Account>>,
        IRequestHandler<FundTransferCommand, ExecutionResult<FundTransferResponse>>
    {
        private readonly IDatabaseConnectionFactory _dbConnectionFactory;
        private readonly ILogger<AccountCommandHandler> _logger;

        /// <summary>
        /// Constructs class instance
        /// </summary>
        /// <param name="dbCconnectionFactory">Connection factory</param>
        /// <param name="logger">Logger instance</param>
        public AccountCommandHandler(IDatabaseConnectionFactory dbCconnectionFactory, ILogger<AccountCommandHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbConnectionFactory = dbCconnectionFactory ?? throw new ArgumentNullException(nameof(dbCconnectionFactory));
        }

        /// <summary>
        /// Handles <c>CreateAccountCommand</c> command
        /// </summary>
        /// <param name="request">The command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of command execution</returns>
        public async Task<ExecutionResult<Account>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            Account account;

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Account == null)
                throw new ArgumentNullException(nameof(request.Account));

            if (request.Account.Balance < 0)
                return new ExecutionResult<Account>(new AccountNotInValidStateDescription("Balance must be equal or great zero"));

            try
            {
                using var connection = await _dbConnectionFactory.CreateConnectionAsync();
                var repository = new AccountRepository(connection);
                account = await repository.Create(request.Account);
            }
            catch (SqlException ex)
            {
                // PK duplication SQL server error number
                if (ex.Number == 2627)
                    return new ExecutionResult<Account>(new AccountNotInValidStateDescription("Account already exist"));
                else
                    throw;
            }

            return new ExecutionResult<Account>(account);
        }

        /// <summary>
        /// Handles <c>FundTransferCommand</c> command
        /// </summary>
        /// <param name="request">The command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Result of command execution 
        /// </returns>
        public async Task<ExecutionResult<FundTransferResponse>> Handle(FundTransferCommand request, CancellationToken cancellationToken)
        {
            bool transferResult = false;
            Account debitAccount = null;
            Account creditAccount = null;
            ExecutionResult<FundTransferResponse> response;

            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var repository = new AccountRepository(connection);
                
            if (IsTransferAllowed(request))
            {
                var updatedAccounts = await repository.TransferAsync(request.AccountNumCredit, request.AccountNumDebit, request.Amount);

                foreach (var item in updatedAccounts)
                {
                    if (item.AccountNum == request.AccountNumDebit)
                        debitAccount = item;
                    else
                        creditAccount = item;

                    transferResult = true;
                }
            }

            if (transferResult == false)
            {
                debitAccount = await repository.ReadById(request.AccountNumDebit);
                creditAccount = await repository.ReadById(request.AccountNumCredit);
            }

            if (debitAccount == null || creditAccount == null)
            {
                response = new ExecutionResult<FundTransferResponse>(new AccountNotExistDescription("One of the accounts is not exist"));
            }
            else
            {
                response = new ExecutionResult<FundTransferResponse>(
                    new FundTransferResponse(creditAccount.AccountNum, creditAccount.Balance,
                                              debitAccount.AccountNum, debitAccount.Balance,
                                              transferResult));
            }

            return response;
        }

        private bool IsTransferAllowed(FundTransferCommand request)
        {
            if (request.AccountNumCredit == request.AccountNumDebit)
                return false;

            if (request.Amount == decimal.Zero)
                return false;

            return true;
        }
    }
}
