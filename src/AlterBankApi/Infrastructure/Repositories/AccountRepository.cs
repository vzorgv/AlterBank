namespace AlterBankApi.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using AlterBankApi.Application.DataModel;

    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public AccountRepository(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            _connection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _transaction = dbTransaction;
        }

        public AccountRepository(IDbConnection dbConnection) 
            : this(dbConnection, null)
        {
        }

        public async Task<IEnumerable<Account>> Read()
        {
            return await _connection.QueryAsync<Account>(@"SELECT * FROM AccountTable", transaction: _transaction);
        }

        public async Task<Account> ReadById(string accountNum)
        {
            var accountSql = @"SELECT * FROM AccountTable WHERE AccountNum = @AccountNum";
            return await _connection.QueryFirstOrDefaultAsync<Account>(accountSql, new { AccountNum = accountNum }, transaction: _transaction);
        }

        public async Task<Account> Create(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            var sql = @"INSERT INTO AccountTable (AccountNum, CurrencyIsoCode, Balance)
                OUTPUT inserted.*
                VALUES (@AccountNum, @CurrencyIsoCode, @Balance)";

            return await _connection.QueryFirstOrDefaultAsync<Account>(sql, param: new { AccountNum = account.AccountNum, CurrencyIsoCode = account.CurrencyIsoCode, Balance = account.Balance }, transaction: _transaction);
        }

        public async Task<Account> Update(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (account.RowVersion == null)
                throw new ArgumentNullException("RowVersion");

            var sql = @"UPDATE AccountTable 
                        SET AccountNum      = @AccountNum,
                            Balance         = @Balance,
                            CurrencyIsoCode = @CurrencyIsoCode
                        OUTPUT inserted.*
                        WHERE AccountNum = @AccountNum
                        AND   RowVersion = @RowVersion";

            var parms = new
            {
                AccountNum = account.AccountNum,
                CurrencyIsoCode = account.CurrencyIsoCode,
                Balance = account.Balance,
                RowVersion = account.RowVersion
            };

            var result = await _connection.QueryFirstOrDefaultAsync<Account>(sql, param: parms, transaction: _transaction);

            if (result == null)
                throw new DBConcurrencyException($"The account {account.AccountNum} you are trying to edit has changed.");

            return result;
        }
    }
}
