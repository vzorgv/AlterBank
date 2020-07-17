namespace AlterBankApi.Application.Requests
{
    using System.Collections.Generic;
    using MediatR;
    using AlterBankApi.Application.Model;
    using AlterBankApi.Application.Responses;

    /// <summary>
    /// Request to get list of accounts
    /// </summary>
    public sealed class GetListOfAccountsRequest : IRequest<ExecutionResult<IEnumerable<Account>>>
    {
        //left empty on purpose
    }
}
