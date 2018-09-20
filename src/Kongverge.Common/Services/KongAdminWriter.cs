using System;
using System.Linq;
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
            Log.Information($"Adding service {service.Name}");
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
            Log.Information("Updating service {name}", service.Name);
            var content = service.ToJsonStringContent();

            try
            {
                var response = await HttpClient.PatchAsync($"/services/{service.Id}", content).ConfigureAwait(false);
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
            Log.Information("Deleting service {id}", serviceId);
            await DeleteRoutes(serviceId).ConfigureAwait(false);

            try
            {
                await HttpClient.DeleteAsync($"/services/{serviceId}").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task AddRoute(string serviceId, KongRoute route)
        {
            Log.Information(@"Adding route {route}", route);
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
            Log.Information("Deleting route {id}", routeId);

            try
            {
                await HttpClient.DeleteAsync($"/routes/{routeId}").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        public async Task UpsertPlugin(KongPlugin plugin)
        {
            Log.Information(string.IsNullOrWhiteSpace(plugin.Id) ? $"Adding plugin {plugin.Name}" : $"Updating plugin {plugin.Name}");
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
            Log.Information("Deleting plugin {id}", pluginId);

            try
            {
                await HttpClient.DeleteAsync($"/plugins/{pluginId}").ConfigureAwait(false);
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
