using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Kongverge.Extension;
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
            services.AddSingleton<KongAdminService>();
            services.AddSingleton<KongvergeWorkflow>();
            services.AddSingleton<ExportWorkflow>();

            var thisAssembly = typeof(ServiceRegistration).Assembly;

            var assemblies = thisAssembly
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Concat(new[] { thisAssembly })
                .Distinct();

            AddPlugins(services, assemblies);

            services.AddSingleton<PluginConverter>();

            services.AddSingleton<IKongPluginCollection>(s => new KongPluginCollection(s.GetServices<IKongPlugin>()));

            services.AddSingleton<IKongAdminReadService, KongAdminReadService>();

            services.AddSingleton<IKongAdminService>(s =>
            {
                var config = s.GetService<IOptions<Settings>>();

                if (config.Value.DryRun)
                {
                    Log.Information("Performing Dry Run.\n\tNo Writes to Kong will occur");
                    return s.GetService<KongAdminDryRun>();
                }

                Log.Information("Performing live integration.\n\tChanges will be made to {host}", config.Value.Admin.Host);
                return s.GetService<KongAdminService>();
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
            services.AddSingleton<HttpClient>();
            services.AddOptions();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            services.Configure<Settings>(set => configuration.Bind(set));
        }

        private static void AddPlugins(IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            var myInterface = typeof(IKongPlugin);

            var types =
                assembliesToScan
                .SelectMany(s => s.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(p => myInterface.IsAssignableFrom(p))
                .Distinct();

            foreach (var t in types)
            {
                services.AddSingleton(myInterface, t);
            }
        }
    }
}
