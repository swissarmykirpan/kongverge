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
    public class KongAdminReadService : IKongAdminReadService
    {
        private const string servicesRoute = "/services/";
        private const string routesRoute = "/routes";
        private readonly Settings _configuration;
        protected readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _settings;
        private readonly IKongPluginCollection _kongPluginCollection;

        public KongAdminReadService(IOptions<Settings> configuration, HttpClient httpClient, IKongPluginCollection kongPluginCollection, PluginConverter converter)
        {
            _configuration = configuration.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri($"http://{_configuration.Admin.Host}:{_configuration.Admin.Port}");

            _settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                Converters = converter != null ? new[] { converter } : null
            };
            _kongPluginCollection = kongPluginCollection;
        }

        public async Task<bool> KongIsReachable()
        {
            try
            {
                var response = await _httpClient.GetAsync("/").ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Error("ERROR: Unable to contact Kong: {host}:{port}", _configuration.Admin.Host, _configuration.Admin.Port);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ERROR: Unable to contact Kong: {host}:{port}", _configuration.Admin.Host, _configuration.Admin.Port);
                return false;
            }

            return true;
        }

        public async Task<List<KongService>> GetServices()
        {
            var services = new List<KongService>();

            var lastPage = false;
            var requestUri = servicesRoute;
            do
            {
                var response = await _httpClient.GetAsync(requestUri).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var servicesResult = JsonConvert.DeserializeObject<GetServicesResponse>(value);

                services.AddRange(servicesResult.Data);
                if (servicesResult.Next == null)
                {
                    lastPage = true;
                }
                else
                {
                    requestUri = servicesResult.Next;
                }
            } while (!lastPage);

            services = await PopulateServiceRoutes(services).ConfigureAwait(false);
            await PopulatePluginInfo(services).ConfigureAwait(false);
            return services;
        }

        private async Task PopulatePluginInfo(List<KongService> services)
        {
            List<PluginBody> plugins = await GetAllPlugins();

            GroupPlugins(services, plugins);
        }

        private async Task<List<PluginBody>> GetAllPlugins()
        {
            var plugins = new List<PluginBody>();
            var lastPage = false;
            var requestUri = "/plugins";
            do
            {
                var response = await _httpClient.GetAsync(requestUri).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var pluginsResult = JsonConvert.DeserializeObject<PluginsResponse>(value, _settings);

                plugins.AddRange(pluginsResult.Data);

                if (pluginsResult.Next == null)
                {
                    lastPage = true;
                }
                else
                {
                    requestUri = pluginsResult.Next;
                }

            } while (!lastPage);
            return plugins;
        }

        private void GroupPlugins(List<KongService> services, List<PluginBody> plugins)
        {
            var serviceGroups = plugins.GroupBy(p => p.service_id).Where(g => !string.IsNullOrEmpty(g.Key)).ToDictionary(g => g.Key, g => g);

            var routeGroups = plugins.GroupBy(p => p.route_id)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToDictionary(g => g.Key, g => g);

            foreach (var service in services)
            {
                if (serviceGroups.ContainsKey(service.Id))
                {
                    service.Extensions = TranslateToConfig(serviceGroups[service.Id]);
                }

                foreach (var route in service.Routes ?? Enumerable.Empty<KongRoute>())
                {
                    if (routeGroups.ContainsKey(route.Id))
                    {
                        route.Extensions = TranslateToConfig(routeGroups[route.Id]);
                    }
                }
            }
        }

        public async Task<List<KongService>> PopulateServiceRoutes(List<KongService> services)
        {
            foreach (var service in services)
            {
                service.Routes = await GetRoutes(service.Name).ConfigureAwait(false);
            }

            return services;
        }

        public async Task<List<KongRoute>> GetRoutes()
        {
            var routes = new List<KongRoute>();
            var lastPage = false;
            var requestUri = routesRoute;
            do
            {
                var response = await _httpClient.GetAsync(requestUri).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var servicesResult = JsonConvert.DeserializeObject<GetRoutesResponse>(value);

                routes.AddRange(servicesResult.Data);

                if (servicesResult.Next == null)
                {
                    lastPage = true;
                }
                else
                {
                    requestUri = servicesResult.Next;
                }
            } while (!lastPage);
            return routes;
        }

        public async Task<List<KongRoute>> GetRoutes(string serviceName)
        {
            var routes = new List<KongRoute>();
            var lastPage = false;
            var requestUri = $"/services/{serviceName}/routes";
            do
            {
                var response = await _httpClient.GetAsync(requestUri).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var servicesResult = JsonConvert.DeserializeObject<GetRoutesResponse>(value);

                routes.AddRange(servicesResult.Data);

                if (servicesResult.Next == null)
                {
                    lastPage = true;
                }
                else
                {
                    requestUri = servicesResult.Next;
                }
            } while (!lastPage);

            return routes;
        }

        public async Task<KongAction<GlobalConfig>> GetGlobalConfig()
        {
            try
            {
                List<PluginBody> plugins = await GetAllPlugins();

                var globalPlugins = plugins.Where(p => null == (p.consumer_id ?? p.service_id ?? p.route_id));

                return
                    KongAction.Success(new GlobalConfig
                    {
                        Extensions = TranslateToConfig(globalPlugins)
                    });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to get all plugins from Kong server");
                throw;
            }
        }

        private IList<IKongPluginConfig> TranslateToConfig(IEnumerable<PluginBody> plugins)
        {
            return plugins.Select(_kongPluginCollection.TranslateToConfig)
                          .Where(p => p != null)
                          .ToList();
        }
    }
}
