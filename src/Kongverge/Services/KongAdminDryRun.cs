using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public Task<KongPluginResponse> UpsertPlugin(PluginBody plugin)
        {
            Log.Information("Adding plugin {plugin}\n\tWith config {config}", plugin.name, plugin.config);
            var response = new KongPluginResponse
            {
                Name = plugin.ToString(),
                Id = Guid.NewGuid().ToString()
            };

            return Task.FromResult(response);
        }

        public Task<KongRoute> AddRoute(KongService service, KongRoute route)
        {
            Log.Information(@"Adding Route
    Route Paths: {path}
    Methods    : {methods}
    Protocols  : {Protocols}", route.Paths, route.Methods, route.Protocols);

            return Task.FromResult(route);
        }

        public Task<KongService> AddService(KongService service)
        {
            Log.Information("Adding service {name}", service.Name);
            service.Id = Guid.NewGuid().ToString();
            return Task.FromResult(service);
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

        public async Task<IEnumerable<KongRoute>> DeleteRoutes(KongService service)
        {
            var routes = await GetRoutes(service.Name).ConfigureAwait(false);
            return await DeleteRoutes(routes).ConfigureAwait(false);
        }

        public Task<IEnumerable<KongRoute>> DeleteRoutes(IEnumerable<KongRoute> routes)
        {
            foreach (var route in routes)
            {
                Log.Information("Deleting route {id} that serves paths {paths}", route.Id, route.Paths);
            }

            return Task.FromResult(routes);
        }

        public Task<string> DeleteService(string serviceId)
        {
            Log.Information("Deleting service {id}", serviceId);
            return Task.FromResult(serviceId);
        }

        public Task<KongService> UpdateService(KongService service)
        {
            Log.Information("Updating service {name} with host {host}", service.Name, service.Host);
            return Task.FromResult(service);
        }
    }
}
