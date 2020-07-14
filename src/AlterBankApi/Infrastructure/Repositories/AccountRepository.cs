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
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public AccountRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _databaseConnectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public AccountRepository(DatabaseContext databaseContext)
        {
            _databaseConnectionFactory = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<IEnumerable<Account>> Read()
        {
            using var connection = await GetConnectionAsync();

            return await connection.QueryAsync<Account>(@"SELECT * FROM AccountTable");
        }

        public async Task<Account> ReadById(string accountNum)
        {
            using var connection = await GetConnectionAsync();
            return await GetAccount(accountNum, connection);
        }

        public async Task<Account> Create(Account account)
        {
            var sql = @"INSERT INTO AccountTable (AccountNum, CurrencyIsoCode, Balance)
                OUTPUT inserted.*
                VALUES (@AccountNum, @CurrencyIsoCode, @Balance)";

            using var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync<Account>(sql, param: new { AccountNum = account.AccountNum, CurrencyIsoCode = account.CurrencyIsoCode, Balance = account.Balance });
        }

        public async Task<Account> Update(Account account)
        {
            if (account.RowVersion == null)
                throw new ArgumentNullException("RowVersion");

            var sql = @"UPDATE AccountTable 
                        SET AccountNum      = @AccountNum,
                            Balance         = @Balance,
                            CurrencyIsoCode = @CurrencyIsoCode
                        OUTPUT inserted.*
                        WHERE AccountNum = @AccountNum
                        AND   RowVersion = @RowVersion";

            using var connection = await GetConnectionAsync();

            var parms = new
            {
                AccountNum = account.AccountNum,
                CurrencyIsoCode = account.CurrencyIsoCode,
                Balance = account.Balance,
                RowVersion = account.RowVersion
            };

            var result = await connection.QueryFirstOrDefaultAsync<Account>(sql, param: parms);

            if (result == null)
                throw new DBConcurrencyException($"The account {account.AccountNum} you are trying to edit has changed.");
            
            return result;
        }

        private async Task<Account> GetAccount(string accountNum, IDbConnection connection, IDbTransaction transaction)
        {
            var accountSql = @"SELECT * FROM AccountTable WHERE AccountNum = @AccountNum";
            return await connection.QueryFirstOrDefaultAsync<Account>(accountSql, new { AccountNum = accountNum }, transaction: transaction);
        }

        private async Task<Account> GetAccount(string accountNum, IDbConnection connection)
        {
            return await GetAccount(accountNum, connection, null);
        }

        private async Task<IDbConnection> GetConnectionAsync()
        {
            return await _databaseConnectionFactory.CreateConnectionAsync();
        }
    }
}
