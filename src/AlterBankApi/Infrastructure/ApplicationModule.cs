namespace AlterBankApi.Infrastructure
{
    using System.Reflection;
    using Autofac;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Modue to configure application parts
    /// </summary>
    public sealed class ApplicationModule : Autofac.Module
    {
        /// <summary>
        /// Loads applcation container config
        /// </summary>
        /// <param name="builder">Builder</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new SqlServerConnectionFactory(c.Resolve<IConfiguration>(), c.Resolve<ILogger<SqlServerConnectionFactory>>()))
                .As<IDatabaseConnectionFactory>();
        }
    }
}
