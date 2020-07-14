namespace AlterBankApi.Infrastructure
{
    using System.Reflection;
    using Autofac;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;

    public sealed class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new SqlServerConnectionFactory(c.Resolve<IConfiguration>(), c.Resolve<ILogger<SqlServerConnectionFactory>>()))
                .As<IDatabaseConnectionFactory>();
        }
    }
}
