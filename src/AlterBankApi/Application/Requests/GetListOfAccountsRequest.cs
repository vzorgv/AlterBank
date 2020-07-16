namespace AlterBankApi.Application.Requests
{
    using System.Collections.Generic;
    using MediatR;
    using AlterBankApi.Application.Model;

    /// <summary>
    /// Request to get list of accounts
    /// </summary>
    public sealed class GetListOfAccountsRequest : IRequest<IEnumerable<Account>>
    {
        //left empty on purpose
    }
}
