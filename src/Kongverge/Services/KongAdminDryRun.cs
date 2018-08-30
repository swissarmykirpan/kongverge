using System;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Kongverge.KongPlugin;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kongverge.Services
{
    public class KongAdminDryRun : KongAdminReader, IKongAdminWriter
    {
        public KongAdminDryRun(
            IOptions<Settings> configuration,
            KongAdminHttpClient httpClient,
            IKongPluginCollection kongPluginCollection,
            PluginConverter converter) : base(configuration, httpClient, kongPluginCollection, converter)
        {
        }

        public Task UpsertPlugin(PluginBody plugin)
        {
            Log.Information("Adding plugin {plugin}\n\tWith config {config}", plugin.name, plugin.config);
            plugin.id = Guid.NewGuid().ToString();
            return Task.CompletedTask;
        }

        public Task AddRoute(KongService service, KongRoute route)
        {
            Log.Information(@"Adding Route
    Route Paths: {path}
    Methods    : {methods}
    Protocols  : {Protocols}", route.Paths, route.Methods, route.Protocols);
            route.Id = Guid.NewGuid().ToString();
            return Task.CompletedTask;
        }

        public Task AddService(KongService service)
        {
            Log.Information("Adding service {name}", service.Name);
            service.Id = Guid.NewGuid().ToString();
            return Task.CompletedTask;
        }

        public Task DeletePlugin(string pluginId)
        {
            Log.Information("Deleting plugin {id}", pluginId);
            return Task.CompletedTask;
        }

        public Task DeleteRoute(string routeId)
        {
            Log.Information("Deleting route {id}", routeId);
            return Task.CompletedTask;
        }

        public Task DeleteService(string serviceId, bool cascadeDeleteRoutes = false)
        {
            Log.Information("Deleting service {id}", serviceId);
            return Task.CompletedTask;
        }

        public Task UpdateService(KongService service)
        {
            Log.Information("Updating service {name} with host {host}", service.Name, service.Host);
            return Task.CompletedTask;
        }
    }
}
