using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Kongverge.Common.Services
{
    public class KongAdminReader : IKongAdminReader
    {
        private const string ConfigurationRoute = "/";
        private const string ServicesRoute = "/services";
        private const string RoutesRoute = "/routes";
        private const string PluginsRoute = "/plugins";

        protected readonly KongAdminHttpClient HttpClient;

        public KongAdminReader(IOptions<Settings> configuration, KongAdminHttpClient httpClient)
        {
            HttpClient = httpClient;
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.BaseAddress = new Uri($"http://{configuration.Value.Admin.Host}:{configuration.Value.Admin.Port}");
            }
        }

        public async Task<bool> KongIsReachable()
        {
            try
            {
                await HttpClient.GetAsync("/").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ERROR: Unable to contact Kong: {baseAddress}", HttpClient.BaseAddress);
                return false;
            }

            return true;
        }

        public async Task<KongConfiguration> GetConfiguration()
        {
            var response = await HttpClient.GetAsync(ConfigurationRoute).ConfigureAwait(false);
            var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<KongConfiguration>(value);
        }

        public async Task<KongService> GetService(string serviceId)
        {
            var response = await HttpClient.GetAsync($"{ServicesRoute}/{serviceId}").ConfigureAwait(false);
            var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<KongService>(value);
        }

        public async Task<IReadOnlyCollection<KongService>> GetServices() =>
            await GetPagedResponse<KongService>(ServicesRoute).ConfigureAwait(false);

        public async Task<IReadOnlyCollection<KongRoute>> GetRoutes() =>
            await GetPagedResponse<KongRoute>(RoutesRoute).ConfigureAwait(false);

        public async Task<IReadOnlyCollection<KongPlugin>> GetPlugins() =>
            await GetPagedResponse<KongPlugin>(PluginsRoute).ConfigureAwait(false);

        protected Task<IReadOnlyList<KongRoute>> GetServiceRoutes(string serviceId) =>
            GetPagedResponse<KongRoute>($"/services/{serviceId}/routes");

        private async Task<IReadOnlyList<T>> GetPagedResponse<T>(string requestUri)
        {
            var data = new List<T>();
            var lastPage = false;

            do
            {
                var response = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
                var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<T>>(value);

                data.AddRange(pagedResponse.Data);

                if (pagedResponse.Next == null)
                {
                    lastPage = true;
                }
                else
                {
                    requestUri = pagedResponse.Next;
                }
            } while (!lastPage);

            return data.AsReadOnly();
        }
    }
}
