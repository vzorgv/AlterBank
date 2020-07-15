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

    public class AccountCommandHandler :
        IRequestHandler<OpenAccountCommand, OpenAccountResponse>,
        IRequestHandler<FundTransferCommand, FundTransferResponse>
    {
        private readonly IDatabaseConnectionFactory _dbConnectionFactory;
        private readonly ILogger<AccountCommandHandler> _logger;

        public AccountCommandHandler(IDatabaseConnectionFactory dbCconnectionFactory, ILogger<AccountCommandHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbConnectionFactory = dbCconnectionFactory ?? throw new ArgumentNullException(nameof(dbCconnectionFactory));
        }

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

        /*
        public async Task<FundTransferResponse> Handle(FundTransferCommand request, CancellationToken cancellationToken)
        {
            //TODO performance degradation
            
            const int RetryCount = 2;
            const int RetryDelayMs = 2;

            FundTransferResponse ret = null;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(RetryDelayMs), retryCount: RetryCount, fastFirst: true);

            var retryPolicy = Policy
                .Handle<DBConcurrencyException>()
                .WaitAndRetryAsync(delay);

            try
            {
                ret = await retryPolicy.ExecuteAsync(async () => await Transfer(request, cancellationToken));
            }
            catch (DBConcurrencyException)
            {
            }

            return await Transfer(request, cancellationToken);
        }
    */

        public async Task<FundTransferResponse> Handle(FundTransferCommand request, CancellationToken cancellationToken)
        {
            bool transferSuccess = false;
            Account debitAccount = null;
            Account creditAccount = null;

            IDbConnection connection;
            IDbTransaction transaction;

            try
            {
                using (connection = await _dbConnectionFactory.CreateConnectionAsync())
                {
                    using (transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        var repository = new AccountRepository(connection, transaction);

                        debitAccount = await repository.ReadById(request.AccountNumDebit);
                        creditAccount = await repository.ReadById(request.AccountNumCredit);

                        var transferAmount = request.Amount;

                        if (IsTransferAllowed(debitAccount, creditAccount, transferAmount))
                        {
                            debitAccount = await UpdateAccountBalance(repository, debitAccount, transferAmount, true);
                            creditAccount = await UpdateAccountBalance(repository, creditAccount, transferAmount, false);

                            transferSuccess = true;
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (DBConcurrencyException)
            {
                // _logger.LogWarning("Concurrency exception while fund transfer.");
                // throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fund transfer error due to databse error.");
                throw;
            }

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

            if (CalcDebitAmount(accountDebit, transferAmount) < 0)
                return false;

            return true;
        }

        private decimal CalcDebitAmount(Account account, decimal transferAmount)
        {
            return account.Balance - transferAmount;
        }

        private decimal CalcCreditAmount(Account account, decimal transferAmount)
        {
            return account.Balance + transferAmount;
        }

        private async Task<Account> UpdateAccountBalance(AccountRepository repository, Account account, decimal transferAmount, bool isDebitAccount)
        {
            if (isDebitAccount)
                account.Balance = CalcDebitAmount(account, transferAmount);
            else
                account.Balance = CalcCreditAmount(account, transferAmount);

            return await repository.Update(account);
        }
    }
}
