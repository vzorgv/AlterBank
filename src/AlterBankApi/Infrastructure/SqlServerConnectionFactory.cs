namespace AlterBankApi.Infrastructure
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implements connection factory to SQL server
    /// </summary>
    public class SqlServerConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;
        private const string ConnectionStringName = "AlterDB";

        private readonly ILogger<SqlServerConnectionFactory> _logger;

        /// <summary>
        /// Constructs connection factory
        /// </summary>
        /// <param name="configuration">The configuration</param>
        /// <param name="logger">The logger</param>
        public SqlServerConnectionFactory(IConfiguration configuration, ILogger<SqlServerConnectionFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            else
            {
                _connectionString = configuration.GetConnectionString(ConnectionStringName);
            }

            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("Conection string to database is empty.");
        }

        /// <summary>
        /// Creates connection asynchoniously
        /// </summary>
        /// <returns></returns>
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            try
            {
                var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                return connection;
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(string.Format("SQL connection timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(string.Format("SQL exception", GetType().FullName), ex);
            }
        }
    }
}
