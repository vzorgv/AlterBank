namespace AlterBankApi.Application.Responses
{
    public sealed class OpenAccountResponse
    {
        public string AccountNum { get; }

        public OpenAccountResponse(string accountNum)
        {
            AccountNum = accountNum;
        }
    }
}
