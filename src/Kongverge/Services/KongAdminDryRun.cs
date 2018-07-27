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
    public class KongAdminDryRun : KongAdminReadService, IKongAdminService
    {
        public KongAdminDryRun(
            IOptions<Settings> configuration,
            HttpClient httpClient,
            IKongPluginCollection kongPluginCollection,
            PluginConverter converter) : base(configuration, httpClient, kongPluginCollection, converter)
        {
        }

        public Task<KongPluginResponse> UpsertPlugin(PluginBody plugin)
        {
            Log.Information("Adding plugin {plugin}\n\tWith config {config}", plugin.name, plugin.config);
            return Task.FromResult(new KongPluginResponse
            {
                Name = plugin.ToString(),
                Id = Guid.NewGuid().ToString()
            });
        }

        public Task<KongAction<KongRoute>> AddRoute(KongService service, KongRoute route)
        {
            Log.Information(@"Adding Route
    Route Paths: {path}
    Methods    : {methods}
    Protocols  : {Protocols}", route.Paths, route.Methods, route.Protocols);

            return Task.FromResult(KongAction.Success(route));
        }

        public Task<KongAction<KongService>> AddService(KongService service)
        {
            Log.Information("Adding service {name}", service.Name);
            service.Id = Guid.NewGuid().ToString();
            return Task.FromResult(KongAction.Success(service));
        }

        public Task<bool> DeletePlugin(string pluginId)
        {
            Log.Information("Deleting plugin {id}", pluginId);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteRoute(KongRoute route)
        {
            Log.Information("Deleting route {id}", route.Id);
            return Task.FromResult(true);
        }

        public async Task<KongAction<IEnumerable<KongRoute>>> DeleteRoutes(KongService service)
        {
            var routes = await GetRoutes(service.Name).ConfigureAwait(false);
            return await DeleteRoutes(routes).ConfigureAwait(false);
        }

        public Task<KongAction<IEnumerable<KongRoute>>> DeleteRoutes(IEnumerable<KongRoute> routes)
        {
            foreach (var route in routes)
            {
                Log.Information("Deleting route {id} that serves paths {paths}", route.Id, route.Paths);
            }

            return Task.FromResult(KongAction.Success(routes));
        }

        public Task<KongAction<KongService>> DeleteService(KongService service)
        {
            Log.Information("Deleting service {name}", service.Name);
            return Task.FromResult(KongAction.Success(service));
        }

        public Task<KongAction<KongService>> UpdateService(KongService service)
        {
            Log.Information("Updating service {name} with host {host}", service.Name, service.Host);
            return Task.FromResult(KongAction.Success(service));
        }
    }
}
