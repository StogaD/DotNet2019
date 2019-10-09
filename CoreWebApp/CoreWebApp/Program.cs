using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CoreWebApp.Serilog;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace CoreWebApp
{
    public class Program
    {
        private static readonly string Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(typeof(Program).Assembly.Location))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.WithExceptionDetails()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    AutoRegisterTemplate = true,
                    MinimumLogEventLevel = LogEventLevel.Information,
                })
                .CreateLogger();

            Log.Information("Start Application{MachineName}");
            Log.Debug("Debug message");
            

            CreateWebHostBuilder(args).Build().Run();

            Log.CloseAndFlush();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(Configuration)
            .ConfigureAppConfiguration((ctx, con) =>
            {
                //disable - work only with valid cert on machine !
                var azureKeyVaultConfigurationProvider_Enabled = false;

                if (azureKeyVaultConfigurationProvider_Enabled)
                {

                    //work only with CurrentUser cert. LocalMachine return 500
                    var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadOnly);
                    var certs = store.Certificates.Find(X509FindType.FindByThumbprint, Configuration["KeyVaultOptions:AzureADCertThumbprint"], false);
                    var cert = certs.OfType<X509Certificate2>().Single();
                    var appId = Configuration["KeyVaultOptions:AzureADApplicationId"];

                    con.AddAzureKeyVault($"https://stogakeyvault.vault.azure.net/",
                                            "d486965c-20e5-4fd4-b51a-2d7e7a9f8a5f",
                                            cert);
                    store.Close();



                }
               




            })
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
