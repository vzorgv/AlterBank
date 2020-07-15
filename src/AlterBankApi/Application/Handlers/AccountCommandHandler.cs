namespace AlterBankApi.Application.Handlers
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Polly;
    using Polly.Contrib.WaitAndRetry;
    using AlterBankApi.Application.Commands;
    using AlterBankApi.Application.DataModel;
    using AlterBankApi.Application.Responses;
    using AlterBankApi.Infrastructure;
    using AlterBankApi.Infrastructure.Repositories;
    using System.Data.Common;
    using Dapper;
    using System.Data.SqlClient;

    /// <summary>
    /// Handles commands which midify account state
    /// </summary>
    public class AccountCommandHandler :
        IRequestHandler<OpenAccountCommand, OpenAccountResponse>,
        IRequestHandler<FundTransferCommand, FundTransferResponse>
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
        /// Handles <c>OpenAccountCommand</c> command
        /// </summary>
        /// <param name="request">The command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of command execution as <c>OpenAccountResponse</c> instance</returns>
        public async Task<OpenAccountResponse> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                AccountNum = request.AccountNum,
                Balance = request.Balance
            };

            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var repository = new AccountRepository(connection);

            var result =  await repository.Create(account);

            return new OpenAccountResponse(result.AccountNum);
        }

        /// <summary>
        /// Handles <c>FundTransferCommand</c> command
        /// </summary>
        /// <param name="request">The command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of command execution as <c>FundTransferResponse</c> instance</returns>
        public async Task<FundTransferResponse> Handle(FundTransferCommand request, CancellationToken cancellationToken)
        {
            //TODO performance degradation
            
            bool transferSuccess = false;
            Account debitAccount = null;
            Account creditAccount = null;

            IDbConnection connection;
            IDbTransaction transaction;

            //TODO consider to use random 
            //var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(10), retryCount: 3, fastFirst: false);

            //var retryPolicy = Policy
            //    .Handle<DBConcurrencyException>()
            //    .WaitAndRetryAsync(delay);

            //return await retryPolicy.ExecuteAsync(async () =>
            try
            {
                using (connection = await _dbConnectionFactory.CreateConnectionAsync())
                {
                    using (transaction = connection.BeginTransaction(IsolationLevel.Snapshot))
                    {
                        var repository = new AccountRepository(connection, transaction);

                        debitAccount = await repository.ReadById(request.AccountNumDebit);
                        creditAccount = await repository.ReadById(request.AccountNumCredit);

                        if (debitAccount != null && creditAccount != null)
                        {
                            var transferAmount = request.Amount;

                            if (IsTransferAllowed(debitAccount, creditAccount, transferAmount))
                            {
                                debitAccount.Balance = CalcBalanceDebit(debitAccount, transferAmount);
                                creditAccount.Balance = CalcBalnceCredit(creditAccount, transferAmount);
                                
                                await repository.UpdateBalancePair(creditAccount, debitAccount);

                                transferSuccess = true;
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (SqlException ex)
            {
                // TODO extract to method
                if (ex.Number != 3960 && ex.State != 5)
                    throw;
            }

            //TODO refactoring return value
            if (creditAccount == null || debitAccount == null)
                    return null;
                else
                    return new FundTransferResponse(creditAccount.AccountNum, creditAccount.Balance,
                        debitAccount.AccountNum, debitAccount.Balance,
                        transferSuccess);
        }

        private bool IsTransferAllowed(Account accountDebit, Account accountCredit, decimal transferAmount)
        {
            if (accountCredit.AccountNum == accountDebit.AccountNum)
                return false;

            if (transferAmount == decimal.Zero)
                return false;

            if (CalcBalanceDebit(accountDebit, transferAmount) < 0)
                return false;

            return true;
        }

        private decimal CalcBalanceDebit(Account account, decimal transferAmount)
        {
            return account.Balance - transferAmount;
        }

        private decimal CalcBalnceCredit(Account account, decimal transferAmount)
        {
            return account.Balance + transferAmount;
        }
    }
}
