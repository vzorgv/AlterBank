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

    public class AccountCommandHandler :
        IRequestHandler<OpenAccountCommand, OpenAccountResponse>,
        IRequestHandler<FundTransferCommand, FundTransferResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountCommandHandler> _logger;

        public AccountCommandHandler(IAccountRepository repository, ILogger<AccountCommandHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accountRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<OpenAccountResponse> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                AccountNum = request.AccountNum,
                CurrencyIsoCode = request.CurrencyIsoCode,
                Balance = request.Balance
            };

            var result =  await _accountRepository.Create(account);

            return new OpenAccountResponse(result.AccountNum);
        }

        public async Task<FundTransferResponse> Handle(FundTransferCommand request, CancellationToken cancellationToken)
        {
            bool transferSuccess = false;
            Account debitAccount;
            Account creditAccount;
            //TODO implement resilency
            /*
                using var connection = await GetConnectionAsync();
                using var transaction = connection.BeginTransaction();
            */

            debitAccount = await _accountRepository.ReadById(request.AccountNumDebit);
            creditAccount = await _accountRepository.ReadById(request.AccountNumCredit);

            try
            {
                var transferAmount = request.Amount;

                if (IsTransferAllowed(debitAccount, creditAccount, transferAmount))
                {
                    debitAccount = await UpdateAccountBalance(debitAccount, transferAmount, true);
                    creditAccount = await UpdateAccountBalance(creditAccount, transferAmount, false);

                    transferSuccess = true;
                }
                // transaction.Rollback();
            }
            catch (DBConcurrencyException)
            {
                // transaction.Abort();
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

        private async Task<Account> UpdateAccountBalance(Account account, decimal transferAmount, bool isDebitAccount)
        {
            if (isDebitAccount)
                account.Balance = CalcDebitAmount(account, transferAmount);
            else
                account.Balance = CalcCreditAmount(account, transferAmount);

            return await _accountRepository.Update(account);
        }
    }
}
