namespace AlterBankApi.Application.Requests
{
    using MediatR;
    using AlterBankApi.Application.Responses;

    public sealed class GetAccountRequest : IRequest<GetAccountResponse>
    {
        public string AccountNum { get; }
        public GetAccountRequest(string accountNum)
        {
            AccountNum = accountNum;
        }
    }
}
