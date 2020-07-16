namespace AlterBankApi.Application.Handlers
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using AlterBankApi.Application.Commands;
    using AlterBankApi.Application.DataModel;
    using AlterBankApi.Application.Responses;
    using AlterBankApi.Infrastructure;
    using AlterBankApi.Infrastructure.Repositories;
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
            FundTransferResponse response = null;
            bool transferResult = false;
            Account debitAccount = null;
            Account creditAccount = null;

            try
            {
                using (var connection = await _dbConnectionFactory.CreateConnectionAsync())
                {
                    using (var transaction = connection.BeginTransaction(IsolationLevel.Snapshot))
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

                                transferResult = true;
                            }
                        }
                        transaction.Commit();
                    }
                }

                response = new FundTransferResponse(creditAccount.AccountNum, creditAccount.Balance,
                                                    debitAccount.AccountNum, debitAccount.Balance,
                                                    transferResult);
            }
            catch (SqlException ex)
            {
                if (IsConcurencySnapshotUpdateException(ex))
                {
                    response = new FundTransferResponse(creditAccount.AccountNum, creditAccount.Balance,
                                                    debitAccount.AccountNum, debitAccount.Balance, false);
                }
                else
                {
                    throw;
                }
            }

            return response;
        }

        private bool IsConcurencySnapshotUpdateException(SqlException exeption)
        {
            return exeption.Number == 3960 && exeption.State == 5;
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
