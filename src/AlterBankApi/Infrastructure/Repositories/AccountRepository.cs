namespace AlterBankApi.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using AlterBankApi.Application.DataModel;

    /// <summary>
    /// Implemnts repository for <c>Account</c> entity
    /// </summary>
    public class AccountRepository : IRepository<Account>
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

            var sql = @"INSERT INTO AccountTable (AccountNum, Balance)
                OUTPUT inserted.*
                VALUES (@AccountNum, @Balance)";

            return await _connection.QueryFirstOrDefaultAsync<Account>(sql, param: new { AccountNum = account.AccountNum, Balance = account.Balance }, transaction: _transaction);
        }

        public async Task<Account> Update(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
 
            var sql = @"UPDATE AccountTable 
                        SET AccountNum      = @AccountNum,
                            Balance         = @Balance
                        OUTPUT inserted.*
                        WHERE AccountNum = @AccountNum";

            var parms = new
            {
                AccountNum = account.AccountNum,
                Balance = account.Balance
            };

            return await _connection.QueryFirstOrDefaultAsync<Account>(sql, param: parms, transaction: _transaction);
        }
    }
}
