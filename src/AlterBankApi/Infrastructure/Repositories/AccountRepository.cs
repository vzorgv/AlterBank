﻿namespace AlterBankApi.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using AlterBankApi.Application.Model;

    /// <summary>
    /// Implemnts repository for <c>Account</c> entity
    /// </summary>
    public class AccountRepository : IRepository<Account>
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        /// <summary>
        /// Constructs the <c>AccountRepositry</c> instance
        /// </summary>
        /// <param name="dbConnection">Connection to database</param>
        /// <param name="dbTransaction">Thet transaction. Can be null if not open explicitly</param>
        public AccountRepository(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            _connection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _transaction = dbTransaction;
        }

        /// <summary>
        /// Constructs the <c>AccountRepositry</c> instance
        /// </summary>
        /// <param name="dbConnection">Connection to database</param>
        public AccountRepository(IDbConnection dbConnection) 
            : this(dbConnection, null)
        {
        }

        /// <summary>
        /// Read opearion
        /// </summary>
        /// <returns>List of Accounts</returns>
        public async Task<IEnumerable<Account>> Read()
        {
            return await _connection.QueryAsync<Account>(@"SELECT * FROM AccountTable", transaction: _transaction);
        }

        /// <summary>
        /// Finds account by its number
        /// </summary>
        /// <param name="accountNum">The account number</param>
        /// <returns>The account or null if not exist</returns>
        public async Task<Account> ReadById(string accountNum)
        {
            var accountSql = @"SELECT * FROM AccountTable WHERE AccountNum = @AccountNum";
            return await _connection.QueryFirstOrDefaultAsync<Account>(accountSql, new { AccountNum = accountNum }, transaction: _transaction);
        }

        /// <summary>
        /// Creates account
        /// </summary>
        /// <param name="account">The account to create</param>
        /// <returns>Newly created account or null if exists</returns>
        public async Task<Account> Create(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            var sql = @"INSERT INTO AccountTable (AccountNum, Balance)
                OUTPUT inserted.*
                VALUES (@AccountNum, @Balance)";

            return await _connection.QueryFirstOrDefaultAsync<Account>(sql, param: new { AccountNum = account.AccountNum, Balance = account.Balance }, transaction: _transaction);
        }

        /// <summary>
        /// Updates account
        /// </summary>
        /// <param name="account">The account</param>
        /// <returns>Updated account</returns>
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

        /// <summary>
        /// Updates pair of accounts at once
        /// </summary>
        /// <param name="accountCredit">Credit account</param>
        /// <param name="accountDebit">Debit account</param>
        /// <returns>None or throws Sql exceptionn </returns>
        public async Task UpdateBalancePair(Account accountCredit, Account accountDebit)
        {
            var parms = new
            {
                nvc_AccountCredit = accountCredit.AccountNum,
                dcml_BalanceCredit = accountCredit.Balance,
                nvc_AccountDebit = accountDebit.AccountNum,
                dcml_BalanceDebit = accountDebit.Balance
            };

            await _connection.ExecuteAsync("UpdateBalancePair", param: parms, transaction: _transaction, commandType: CommandType.StoredProcedure);
        }
    }
}
