namespace AlterBankApi.Infrastructure
{
    using System;
    using System.Data;
    using System.Threading.Tasks;

    public class DatabaseContext
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private IDbConnection dbConnection;

        public DatabaseContext(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public async Task<IDbConnection> GetConnectionAsync()
        {
            return await _databaseConnectionFactory.CreateConnectionAsync();
        }
    }
}
