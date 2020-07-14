namespace AlterBankApi.Infrastructure
{
    using System.Data;
    using System.Threading.Tasks;

    public interface IDatabaseConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}
