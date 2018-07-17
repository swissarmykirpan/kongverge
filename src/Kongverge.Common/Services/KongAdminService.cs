using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Extension;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Kongverge.Common.Services
{
    public class KongAdminService : KongAdminReadService, IKongAdminService
    {
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

        public KongAdminService(
            IOptions<Settings> configuration,
            HttpClient httpClient,
            IExtensionCollection extensionCollection,
            PluginConverter converter) : base(configuration, httpClient, extensionCollection, converter)
        {
        }

        #region Services

        public async Task<KongAction<KongService>> AddService(KongService service)
        {
            var content = ToJsonContent(service);

            try
            {
                var response = await _httpClient.PostAsync("/services/", content).ConfigureAwait(false);
                bool success = response.StatusCode == HttpStatusCode.Created;
                if (success)
                {
                    var apiservice = JsonConvert.DeserializeObject<KongService>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                    // Populate the Id from Kong, because we don't have it.
                    service.MergeFromService(apiservice);

                    return KongAction.Success(service);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }

            return KongAction.Failure<KongService>();
        }

        private StringContent ToJsonContent<T>(T service)
        {
            var json = JsonConvert.SerializeObject(service, _jsonSettings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public async Task<KongAction<KongService>> UpdateService(KongService service)
        {
            Log.Information("Updating service {name} to config {data}", service.Name, service);
            var requestUri = new Uri($"/services/{service.Name}", UriKind.Relative);
            var method = new HttpMethod("PATCH");

            var content = ToJsonContent(service);
            var request = new HttpRequestMessage(method, requestUri) { Content = content };

            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var updated = JsonConvert.DeserializeObject<KongService>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                    service.MergeFromService(updated);

                    return KongAction.Success(service);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }

            return KongAction.Failure<KongService>();
        }

        public async Task<KongAction<KongService>> DeleteService(KongService service)
        {
            var requestUri = $"/services/{service}";

            var method = new HttpMethod("DELETE");

            var request = new HttpRequestMessage(method, requestUri);
            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return KongAction.Success(service);
                }
                else
                {
                    return KongAction.Failure<KongService>();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        #endregion

        #region Routes

        public async Task<KongAction<KongRoute>> AddRoute(KongService service, KongRoute route)
        {
            Log.Information(@"Adding Route
    Route Paths: {path}
    Methods    : {methods}
    Protocols  : {Protocols}", route.Hosts, route.Paths);

            var content = ToJsonContent(route);

            try
            {
                var result = await _httpClient.PostAsync($"/services/{service.Id ?? service.Name}/routes", content).ConfigureAwait(false);
                if (result.StatusCode == HttpStatusCode.Created)
                {
                    var addedRoute = JsonConvert.DeserializeObject<KongRoute>(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

                    route.MergeFromService(addedRoute);

                    Log.Information("Added route {route}", addedRoute);
                    return KongAction.Success(route);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }

            return KongAction.Failure<KongRoute>();
        }

        public async Task<KongAction<IEnumerable<KongRoute>>> DeleteRoutes(KongService service)
        {
            IEnumerable<KongRoute> routes = await GetRoutes(service.Name).ConfigureAwait(false);
            await Task.WhenAll(routes.Select(DeleteRoute)).ConfigureAwait(false);

            return KongAction.Success(routes);
        }

        public async Task<bool> DeleteRoute(KongRoute route)
        {
            var requestUri = $"/routes/{route.Id}";

            var method = new HttpMethod("DELETE");

            var request = new HttpRequestMessage(method, requestUri);
            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        #endregion

        #region Plugins

        public async Task<KongPluginResponse> UpsertPlugin(PluginBody plugin)
        {
            var content = ToJsonContent(plugin);

            try
            {
                var response = await _httpClient.PutAsync("/plugins", content).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.Created
                    && response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Error(@"Failed to add plugin {name} to target {id}.
Error was:
{content}", plugin.name, plugin.service_id ?? plugin.consumer_id ?? plugin.route_id, await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                    return new KongPluginResponse();
                }

                var pluginResponse = JsonConvert.DeserializeObject<KongPluginResponse>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                Log.Information("Successfully added plugin {name} to route {id}", plugin.name, plugin.consumer_id ?? plugin.service_id ?? plugin.route_id);

                return pluginResponse;
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

            var method = new HttpMethod("DELETE");

            var request = new HttpRequestMessage(method, requestUri);
            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                Log.Information("Called delete for plugin {id}, result {response}", pluginId, response.StatusCode);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        #endregion
    }
}
