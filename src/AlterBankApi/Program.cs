namespace AlterBankApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;

    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The basic namespace
        /// </summary>
        public static readonly string Namespace = typeof(Program).Namespace;
        
        /// <summary>
        /// The application name
        /// </summary>
        public static readonly string AppName = Namespace;

        private const string AppSettingsFileName = "appsettings.json";
        private const string AppContextLogName = "ApplicationContext";

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">args</param>
        /// <returns>result</returns>
        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = BuildHost(configuration, args);

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHost BuildHost(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .Build();

         private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty(AppContextLogName, AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(AppSettingsFileName, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
