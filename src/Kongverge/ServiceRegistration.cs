using System.IO;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Kongverge.Common.Workflow;
using Kongverge.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kongverge
{
    public static class ServiceRegistration
    {
        public static void CreateConsoleLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("Starting up");
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddSingleton<KongAdminDryRun>();
            services.AddSingleton<KongAdminWriter>();
            services.AddSingleton<KongvergeWorkflow>();
            services.AddSingleton<ExportWorkflow>();

            services.AddSingleton<IKongAdminReader, KongAdminReader>();

            services.AddSingleton<IKongAdminWriter>(s =>
            {
                var config = s.GetService<IOptions<Settings>>();

                if (config.Value.DryRun)
                {
                    Log.Information("Performing Dry Run.\n\tNo Writes to Kong will occur");
                    return s.GetService<KongAdminDryRun>();
                }

                Log.Information("Performing live integration.\n\tChanges will be made to {host}", config.Value.Admin.Host);
                return s.GetService<KongAdminWriter>();
            });

            services.AddSingleton<Workflow>(s =>
            {
                var config = s.GetService<IOptions<Settings>>();

                if (string.IsNullOrEmpty(config.Value.OutputFolder))
                {
                    Log.Information("Performing full diff and merge.");
                    return s.GetService<KongvergeWorkflow>();
                }

                Log.Information("Exporting information from Kong");
                return s.GetService<ExportWorkflow>();
            });

            services.AddSingleton<IDataFileHelper, DataFileHelper>();
            services.AddSingleton<KongAdminHttpClient>();
            services.AddOptions();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .Build();
            services.Configure<Settings>(set => configuration.Bind(set));
        }
    }
}
