using System;
using System.Collections.Generic;
using System.Linq;
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
    public class KongAdminReader : IKongAdminReader
    {
        private const string ConfigurationRoute = "/";
        private const string ServicesRoute = "/services";
        private const string RoutesRoute = "/routes";
        private const string PluginsRoute = "/plugins";

        protected readonly KongAdminHttpClient HttpClient;
        private readonly IKongPluginCollection _kongPluginCollection;

        public KongAdminReader(IOptions<Settings> configuration, KongAdminHttpClient httpClient, IKongPluginCollection kongPluginCollection, PluginConverter converter)
        {
            HttpClient = httpClient;
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.BaseAddress = new Uri($"http://{configuration.Value.Admin.Host}:{configuration.Value.Admin.Port}");
            }

            _kongPluginCollection = kongPluginCollection;
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
            response.EnsureSuccessStatusCode();

            var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var configurationResult = JsonConvert.DeserializeObject<KongConfiguration>(value);

            return configurationResult;
        }

        public async Task<IReadOnlyCollection<KongService>> GetServices()
        {
            var services = await GetPagedResponse<KongService>(ServicesRoute).ConfigureAwait(false);

            await PopulateServiceRoutes(services).ConfigureAwait(false);
            await PopulatePluginInfo(services).ConfigureAwait(false);

            return services;
        }

        public async Task<KongService> GetService(string serviceId)
        {
            var requestUri = $"{ServicesRoute}/{serviceId}";
            var response = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var service = JsonConvert.DeserializeObject<KongService>(value);

            var plugins = await GetServicePlugins(service.Id).ConfigureAwait(false);
            service.Plugins = TranslateToConfig(plugins);

            service.Routes = await GetRoutes(service.Id).ConfigureAwait(false);

            foreach (var route in service.Routes)
            {
                var pluginsRead = await GetRoutePlugins(route.Id).ConfigureAwait(false);
                route.Plugins = TranslateToConfig(pluginsRead);
            }

            return service;
        }

        private Task<IReadOnlyCollection<PluginBody>> GetServicePlugins(string serviceId)
        {
            return GetPagedResponse<PluginBody>($"{PluginsRoute}?service_id={serviceId}");
        }

        private Task<IReadOnlyCollection<PluginBody>> GetRoutePlugins(string routeId)
        {
            return GetPagedResponse<PluginBody>($"{PluginsRoute}?route_id={routeId}");
        }

        private async Task PopulatePluginInfo(IReadOnlyCollection<KongService> services)
        {
            var plugins = await GetAllPlugins().ConfigureAwait(false);

            GroupPlugins(services, plugins);
        }

        private Task<IReadOnlyCollection<PluginBody>> GetAllPlugins()
        {
            return GetPagedResponse<PluginBody>(PluginsRoute);
        }

        private void GroupPlugins(IReadOnlyCollection<KongService> services, IReadOnlyCollection<PluginBody> plugins)
        {
            var serviceGroups = plugins.GroupBy(p => p.service_id).Where(g => !string.IsNullOrEmpty(g.Key)).ToDictionary(g => g.Key, g => g);

            var routeGroups = plugins.GroupBy(p => p.route_id)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToDictionary(g => g.Key, g => g);

            foreach (var service in services)
            {
                if (serviceGroups.ContainsKey(service.Id))
                {
                    service.Plugins = TranslateToConfig(serviceGroups[service.Id]);
                }

                foreach (var route in service.Routes ?? Enumerable.Empty<KongRoute>())
                {
                    if (routeGroups.ContainsKey(route.Id))
                    {
                        route.Plugins = TranslateToConfig(routeGroups[route.Id]);
                    }
                }
            }
        }

        public async Task PopulateServiceRoutes(IReadOnlyCollection<KongService> services)
        {
            foreach (var service in services)
            {
                service.Routes = await GetRoutes(service.Id).ConfigureAwait(false);
            }
        }

        public Task<IReadOnlyCollection<KongRoute>> GetRoutes(string serviceId)
        {
            return GetPagedResponse<KongRoute>($"/services/{serviceId}/routes");
        }

        public async Task<GlobalConfig> GetGlobalConfig()
        {
            var plugins = await GetAllPlugins().ConfigureAwait(false);

            var globalPlugins = plugins.Where(p => null == (p.consumer_id ?? p.service_id ?? p.route_id));

            return new GlobalConfig
            {
                Plugins = TranslateToConfig(globalPlugins)
            };
        }

        private IReadOnlyCollection<IKongPluginConfig> TranslateToConfig(IEnumerable<PluginBody> plugins)
        {
            return plugins.Select(_kongPluginCollection.TranslateToConfig)
                          .Where(p => p != null)
                          .ToArray();
        }

        private async Task<IReadOnlyCollection<T>> GetPagedResponse<T>(string requestUri)
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
