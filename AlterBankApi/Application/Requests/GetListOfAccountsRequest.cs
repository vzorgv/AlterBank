namespace AlterBankApi.Application.Requests
{
    using System.Collections.Generic;
    using MediatR;
    using AlterBankApi.Application.Responses;

    public sealed class GetListOfAccountsRequest : IRequest<IEnumerable<GetAccountResponse>>
    {
        //left empty on purpose
    }
}
