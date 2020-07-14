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
    using AlterBankApi.Infrastructure.Repositories;

    public class AccountRequestHandler :
        IRequestHandler<GetAccountRequest, GetAccountResponse>,
        IRequestHandler<GetListOfAccountsRequest, IEnumerable<GetAccountResponse>>
    {
        private readonly ILogger<AccountRequestHandler> _logger;
        private readonly IAccountRepository _accountRepository;

        public AccountRequestHandler(IAccountRepository repository, ILogger<AccountRequestHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accountRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEnumerable<GetAccountResponse>> Handle(GetListOfAccountsRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountRepository.Read();

            return result
                .Select(account => new GetAccountResponse(account))
                .ToList();
        }

        public async Task<GetAccountResponse> Handle(GetAccountRequest request, CancellationToken cancellationToken)
        {
            var result =  await _accountRepository.ReadById(request.AccountNum);
            return result == null ? null : new GetAccountResponse(result);
        }
    }
}
