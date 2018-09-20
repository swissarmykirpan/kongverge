using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.DTOs;
using Kongverge.Helpers;
using Kongverge.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kongverge.Workflow
{
    public abstract class Workflow
    {
        protected Workflow(IKongAdminReader kongReader, IOptions<Settings> configuration)
        {
            KongReader = kongReader;
            Configuration = configuration.Value;
        }

        protected IKongAdminReader KongReader { get; }
        protected Settings Configuration { get; }

        public async Task<int> Execute()
        {
            Log.Information("Getting existing services from {host}", Configuration.Admin.Host);

            var reachable = await KongReader.KongIsReachable();
            if (!reachable)
            {
                return ExitWithCode.Return(ExitCode.HostUnreachable);
            }

            try
            {
                return await DoExecute();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while process job");
                throw;
            }
        }

        protected async Task<KongvergeConfiguration> GetExistingConfiguration()
        {
            var plugins = await KongReader.GetPlugins();
            var services = await KongReader.GetServices();
            var routes = await KongReader.GetRoutes();

            foreach (var existingService in services)
            {
                PopulateServiceTree(existingService, routes, plugins);
            }

            return new KongvergeConfiguration
            {
                Services = services,
                GlobalConfig = new ExtendibleKongObject
                {
                    Plugins = plugins.Where(x => x.IsGlobal()).ToArray()
                }
            };
        }

        private void PopulateServiceTree(
            KongService service,
            IReadOnlyCollection<KongRoute> routes,
            IReadOnlyCollection<KongPlugin> plugins)
        {
            service.Plugins = plugins.Where(x => x.ServiceId == service.Id).ToArray();
            service.Routes = routes.Where(x => x.Service.Id == service.Id).ToArray();
            foreach (var serviceRoute in service.Routes)
            {
                serviceRoute.Plugins = plugins.Where(x => x.RouteId == serviceRoute.Id).ToArray();
            }
        }

        public abstract Task<int> DoExecute();
    }
}
