namespace AlterBankApi.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using AlterBankApi.Application.DataModel;

    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> Read();

        Task<Account> ReadById(string accountNum);

        Task<Account> Create(Account account);

        Task<Account> Update(Account account);
    }
}
