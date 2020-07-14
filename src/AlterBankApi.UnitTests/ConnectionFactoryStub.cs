using AlterBankApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace AlterBankApi.UnitTests
{
    internal sealed class ConnectionFactoryStub : IDatabaseConnectionFactory
    {
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            return await Task.FromResult<IDbConnection>(null);
        }
    }
}
