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
    using AlterBankApi.Infrastructure.Repositories;
    using AlterBankApi.Infrastructure;

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
                CurrencyIsoCode = request.CurrencyIsoCode,
                Balance = request.Balance
            };

            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var repository = new AccountRepository(connection);

            var result =  await repository.Create(account);

            return new OpenAccountResponse(result.AccountNum);
        }

        public async Task<FundTransferResponse> Handle(FundTransferCommand request, CancellationToken cancellationToken)
        {
            bool transferSuccess = false;
            Account debitAccount;
            Account creditAccount;

            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            var repository = new AccountRepository(connection, transaction);

            debitAccount = await repository.ReadById(request.AccountNumDebit);
            creditAccount = await repository.ReadById(request.AccountNumCredit);

            try
            {
                var transferAmount = request.Amount;

                if (IsTransferAllowed(debitAccount, creditAccount, transferAmount))
                {
                    debitAccount = await UpdateAccountBalance(repository, debitAccount, transferAmount, true);
                    creditAccount = await UpdateAccountBalance(repository, creditAccount, transferAmount, false);

                    transferSuccess = true;
                }
                transaction.Commit();
            }
            catch (DBConcurrencyException)
            {
                transaction.Rollback();
                _logger.LogWarning("Concurrency exception while fund transfer.");
            }

            return new FundTransferResponse(creditAccount.AccountNum, creditAccount.Balance, 
                debitAccount.AccountNum, debitAccount.Balance,
                transferSuccess);
        }

        private bool IsTransferAllowed(Account accountDebit, Account accountCredit, decimal transferAmount)
        {
            if (CalcDebitAmount(accountDebit, transferAmount) < 0)
                return false;

            if (accountCredit.CurrencyIsoCode != accountDebit.CurrencyIsoCode)
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
