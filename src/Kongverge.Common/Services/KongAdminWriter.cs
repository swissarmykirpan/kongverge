using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Kongverge.Common.Services
{
    public class KongAdminWriter : KongAdminReader, IKongAdminWriter
    {
        public KongAdminWriter(IOptions<Settings> configuration, KongAdminHttpClient httpClient)
            : base(configuration, httpClient)
        {
        }

        public async Task AddService(KongService service)
        {
            var content = service.ToJsonStringContent();

            try
            {
                var response = await HttpClient.PostAsync("/services/", content).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var added = JsonConvert.DeserializeObject<KongService>(responseBody);
                service.Id = added.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task UpdateService(KongService service)
        {
            var requestUri = new Uri($"/services/{service.Name}", UriKind.Relative);
            var content = service.ToJsonStringContent();
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            };

            try
            {
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var updated = JsonConvert.DeserializeObject<KongService>(responseBody);
                service.Id = updated.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task DeleteService(string serviceId)
        {
            await DeleteRoutes(serviceId).ConfigureAwait(false);

            var requestUri = $"/services/{serviceId}";
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

        public async Task AddRoute(string serviceId, KongRoute route)
        {
            var content = route.ToJsonStringContent();

            try
            {
                var response = await HttpClient.PostAsync($"/services/{serviceId}/routes", content).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var added = JsonConvert.DeserializeObject<KongRoute>(responseBody);
                route.Id = added.Id;
                route.Service = added.Service;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
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

        public async Task UpsertPlugin(KongPlugin plugin)
        {
            var content = plugin.ToJsonStringContent();

            try
            {
                var response = await HttpClient.PutAsync("/plugins", content).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var updated = JsonConvert.DeserializeObject<KongPlugin>(responseBody);
                plugin.Id = updated.Id;
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
                await HttpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        private async Task DeleteRoutes(string serviceId)
        {
            var routes = await GetServiceRoutes(serviceId).ConfigureAwait(false);
            await Task.WhenAll(routes.Select(x => DeleteRoute(x.Id))).ConfigureAwait(false);
        }
    }
}
