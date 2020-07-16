namespace AlterBankApi.Application.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using AlterBankApi.Application.Model;
    using AlterBankApi.Application.Requests;
    using AlterBankApi.Application.Responses;
    using AlterBankApi.Infrastructure;
    using AlterBankApi.Infrastructure.Repositories;

    /// <summary>
    /// Handler of requests to Account
    /// </summary>
    public class AccountRequestHandler :
        IRequestHandler<GetAccountRequestById, Account>,
        IRequestHandler<GetListOfAccountsRequest, IEnumerable<Account>>
    {
        private readonly IDatabaseConnectionFactory _dbConnectionFactory;

        /// <summary>
        /// Constructs <c>AccountRequestHandler</c> instance
        /// </summary>
        /// <param name="dbCconnectionFactory">Connection factory to database</param>
        public AccountRequestHandler(IDatabaseConnectionFactory dbCconnectionFactory)
        {
            _dbConnectionFactory = dbCconnectionFactory ?? throw new ArgumentNullException(nameof(dbCconnectionFactory));
        }

        /// <summary>
        /// Handles the <c>GetListOfAccountsRequest</c> request asynchroniously
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The list account accounts</returns>
        public async Task<IEnumerable<Account>> Handle(GetListOfAccountsRequest request, CancellationToken cancellationToken)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var repository = new AccountRepository(connection);

            var result = await repository.Read();

            return result.ToList();
        }

        /// <summary>
        /// Handles the <c>GetAccountRequestById</c> request asynchroniously
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The account if exit or null otherwise</returns>
        public async Task<Account> Handle(GetAccountRequestById request, CancellationToken cancellationToken)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var repository = new AccountRepository(connection);

            return await repository.ReadById(request.AccountNum);
        }
    }
}
