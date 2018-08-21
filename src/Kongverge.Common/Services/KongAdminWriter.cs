using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            HttpClient httpClient,
            IKongPluginCollection kongPluginCollection,
            PluginConverter converter) : base(configuration, httpClient, kongPluginCollection, converter)
        {
        }

        public async Task<KongAction<KongService>> AddService(KongService service)
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

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Log.Information("Failed to add service {service} : {errorMessage}", service, responseBody);
                    return KongAction.Failure<KongService>(response.StatusCode, responseBody);
                }

                var success = response.StatusCode == HttpStatusCode.Created;
                if (success)
                {
                    var apiservice = JsonConvert.DeserializeObject<KongService>(responseBody);

                    // Populate the Id from Kong, because we don't have it.
                    service.MergeFromService(apiservice);

                    return KongAction.Success(response.StatusCode, service);
                }

                return KongAction.Failure<KongService>(response.StatusCode, responseBody);

            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<KongAction<KongService>> UpdateService(KongService service)
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

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var updated = JsonConvert.DeserializeObject<KongService>(responseBody);

                    service.MergeFromService(updated);

                    return KongAction.Success(response.StatusCode, service);
                }

                return KongAction.Failure<KongService>(response.StatusCode, responseBody);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<KongAction<string>> DeleteService(string serviceId)
        {
            var requestUri = $"/services/{serviceId}";

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            try
            {
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return KongAction.Success(serviceId);
                }

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return KongAction.Failure<string>(response.StatusCode, responseBody);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<KongAction<KongRoute>> AddRoute(KongService service, KongRoute route)
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

                if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                    Log.Information("Failed to add route {route} : {errorMessage}", route, responseBody);
                    return KongAction.Failure<KongRoute>(result.StatusCode, responseBody);
                }
                if (result.StatusCode == HttpStatusCode.Created)
                {
                    var addedRoute = JsonConvert.DeserializeObject<KongRoute>(responseBody);

                    route.MergeFromService(addedRoute);

                    Log.Information("Added route {route}", addedRoute);
                    return KongAction.Success(result.StatusCode, route);
                }

                return KongAction.Failure<KongRoute>(result.StatusCode, responseBody);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<KongAction<IEnumerable<KongRoute>>> DeleteRoutes(KongService service)
        {
            IEnumerable<KongRoute> routes = await GetRoutes(service.Name).ConfigureAwait(false);
            await Task.WhenAll(routes.Select(r => DeleteRoute(r.Id))).ConfigureAwait(false);

            return KongAction.Success(routes);
        }

        public async Task<bool> DeleteRoute(string routeId)
        {
            var requestUri = $"/routes/{routeId}";

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            try
            {
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<KongAction<KongPluginResponse>> UpsertPlugin(PluginBody plugin)
        {
            var content = KongJsonConvert.Serialize(plugin);

            try
            {
                var response = await HttpClient.PutAsync("/plugins", content).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.Created
                    && response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Error(@"Failed to add plugin {name} to target {id}.
Error was:
{content}", plugin.name, plugin.service_id ?? plugin.consumer_id ?? plugin.route_id, responseBody);

                    return KongAction.Failure<KongPluginResponse>(response.StatusCode, responseBody);
                }

                var pluginResponse = JsonConvert.DeserializeObject<KongPluginResponse>(responseBody);

                Log.Information("Successfully added plugin {name} to route {id}", plugin.name, plugin.consumer_id ?? plugin.service_id ?? plugin.route_id);

                return KongAction.Success(response.StatusCode, pluginResponse);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task<bool> DeletePlugin(string pluginId)
        {
            var requestUri = $"/plugins/{pluginId}";

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            try
            {
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                Log.Information("Called delete for plugin {id}, result {response}", pluginId, response.StatusCode);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }
    }
}
