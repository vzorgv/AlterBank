namespace AlterBankApi.Infrastructure
{
    using System.Data;
    using System.Threading.Tasks;

    /// <summary>
    /// Declares factory to create connection to database
    /// </summary>
    public interface IDatabaseConnectionFactory
    {
        /// <summary>
        /// Creates connection to database
        /// </summary>
        /// <returns>The <c>IDbConnection</c> implementation</returns>
        Task<IDbConnection> CreateConnectionAsync();
    }
}
