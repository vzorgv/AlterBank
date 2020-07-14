namespace AlterBankApi.Application.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using AlterBankApi.Application.Requests;
    using AlterBankApi.Application.Responses;
    using AlterBankApi.Infrastructure;
    using AlterBankApi.Infrastructure.Repositories;

    public class AccountRequestHandler :
        IRequestHandler<GetAccountRequest, GetAccountResponse>,
        IRequestHandler<GetListOfAccountsRequest, IEnumerable<GetAccountResponse>>
    {
        private readonly IDatabaseConnectionFactory _dbConnectionFactory;
        private readonly ILogger<AccountRequestHandler> _logger;

        public AccountRequestHandler(IDatabaseConnectionFactory dbCconnectionFactory, ILogger<AccountRequestHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbConnectionFactory = dbCconnectionFactory ?? throw new ArgumentNullException(nameof(dbCconnectionFactory));
        }

        public async Task<IEnumerable<GetAccountResponse>> Handle(GetListOfAccountsRequest request, CancellationToken cancellationToken)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var repository = new AccountRepository(connection);

            var result = await repository.Read();

            return result
                .Select(account => new GetAccountResponse(account))
                .ToList();
        }

        public async Task<GetAccountResponse> Handle(GetAccountRequest request, CancellationToken cancellationToken)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var repository = new AccountRepository(connection);

            var result =  await repository.ReadById(request.AccountNum);
            return result == null ? null : new GetAccountResponse(result);
        }
    }
}
