using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.KongPlugin;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Kongverge.Common.Services
{
    public class KongAdminWriter : KongAdminReader, IKongAdminWriter
    {
        public KongAdminWriter(
            IOptions<Settings> configuration,
            KongAdminHttpClient httpClient,
            IKongPluginCollection kongPluginCollection,
            PluginConverter converter) : base(configuration, httpClient, kongPluginCollection, converter)
        {
        }

        public async Task<KongService> AddService(KongService service)
        {
            var routes = service.Routes;
            var plugins = service.Plugins;
            service.Routes = null;
            service.Plugins = null;
            var content = KongJsonConvert.Serialize(service);
            service.Routes = routes;
            service.Plugins = plugins;

            try
            {
                var response = await HttpClient.PostAsync("/services/", content).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiservice = JsonConvert.DeserializeObject<KongService>(responseBody);
                service.MergeFromService(apiservice);
                return service;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<KongService> UpdateService(KongService service)
        {
            Log.Information("Updating service {name} to config {data}", service.Name, service);
            var requestUri = new Uri($"/services/{service.Name}", UriKind.Relative);

            var routes = service.Routes;
            var plugins = service.Plugins;
            service.Routes = null;
            service.Plugins = null;
            var content = KongJsonConvert.Serialize(service);
            service.Routes = routes;
            service.Plugins = plugins;

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            };

            try
            {
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var updated = JsonConvert.DeserializeObject<KongService>(responseBody);
                service.MergeFromService(updated);
                return service;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<string> DeleteService(string serviceId)
        {
            var requestUri = $"/services/{serviceId}";

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            try
            {
                await HttpClient.SendAsync(request).ConfigureAwait(false);
                return serviceId;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<KongRoute> AddRoute(KongService service, KongRoute route)
        {
            Log.Information(@"Adding Route
    Route Paths: {paths}
    Methods    : {methods}
    Protocols  : {protocols}",
                route.Paths, route.Methods, route.Protocols);

            var plugins = route.Plugins;
            route.Plugins = null;
            var content = KongJsonConvert.Serialize(route);
            route.Plugins = plugins;

            try
            {
                var result = await HttpClient.PostAsync($"/services/{service.Id ?? service.Name}/routes", content).ConfigureAwait(false);
                var responseBody = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                var addedRoute = JsonConvert.DeserializeObject<KongRoute>(responseBody);
                route.MergeFromRoute(addedRoute);
                Log.Information("Added route {route}", addedRoute);
                return route;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<KongRoute>> DeleteRoutes(KongService service)
        {
            IEnumerable<KongRoute> routes = await GetRoutes(service.Name).ConfigureAwait(false);
            await Task.WhenAll(routes.Select(r => DeleteRoute(r.Id))).ConfigureAwait(false);
            return routes;
        }

        public async Task DeleteRoute(string routeId)
        {
            var requestUri = $"/routes/{routeId}";

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            try
            {
                await HttpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<KongPluginResponse> UpsertPlugin(PluginBody plugin)
        {
            var content = KongJsonConvert.Serialize(plugin);

            try
            {
                var response = await HttpClient.PutAsync("/plugins", content).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var pluginResponse = JsonConvert.DeserializeObject<KongPluginResponse>(responseBody);
                Log.Information("Successfully added plugin {name} to route {id}", plugin.name, plugin.consumer_id ?? plugin.service_id ?? plugin.route_id);
                return pluginResponse;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task DeletePlugin(string pluginId)
        {
            var requestUri = $"/plugins/{pluginId}";

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            try
            {
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                Log.Information("Called delete for plugin {id}, result {response}", pluginId, response.StatusCode);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }
    }
}
