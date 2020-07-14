namespace AlterBankApi.Infrastructure
{
    using System.Reflection;
    using Autofac;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using AlterBankApi.Application.Handlers;
    using MediatR;
    using AlterBankApi.Infrastructure.Repositories;

    public sealed class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new SqlServerConnectionFactory(c.Resolve<IConfiguration>(), c.Resolve<ILogger<SqlServerConnectionFactory>>()))
                .As<IDatabaseConnectionFactory>();

            builder.Register(c => new AccountRepository(c.Resolve<IDatabaseConnectionFactory>()))
                .As<IAccountRepository>();
        }
    }
}
