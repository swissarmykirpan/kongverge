using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Kongverge.KongPlugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kongverge.Integration.Tests
{
    public class KongvergeTestFixture : IDisposable
    {
        public const string TestServiceNamePrefix = "testservice_";

        private readonly ServiceProvider _serviceProvider;
        private readonly IList<KongService> _cleanUp;
        private readonly IKongAdminWriter _kongAdminWriter;

        public IDataFileHelper DataFileHelper => _serviceProvider.GetService<IDataFileHelper>();
        public IKongAdminReader KongAdminReader => _serviceProvider.GetService<IKongAdminReader>();
        public Settings Settings => _serviceProvider.GetRequiredService<IOptions<Settings>>().Value;
        public IKongPluginCollection PluginCollection => new KongPluginCollection(_serviceProvider.GetServices<IKongPlugin>());
        public ServiceBuilder ServiceBuilder = new ServiceBuilder(TestServiceNamePrefix);

        public KongvergeTestFixture()
        {
            var services = new ServiceCollection();
            ServiceRegistration.AddServices(services);
            _serviceProvider = services.BuildServiceProvider();
            _cleanUp = new List<KongService>();
            _kongAdminWriter = _serviceProvider.GetService<IKongAdminWriter>();
            DeleteExistingTestServices().GetAwaiter().GetResult();
            DeleteExistingGlobalPlugins().GetAwaiter().GetResult();
        }

        private async Task<bool> DeleteExistingGlobalPlugins()
        {
            var globalConfig = await KongAdminReader.GetGlobalConfig();
            foreach (var plugin in globalConfig.Plugins)
            {
                await _kongAdminWriter.DeletePlugin(plugin.id);
            }

            return true;
        }

        private async Task<bool> DeleteExistingTestServices()
        {
            var services = await KongAdminReader.GetServices();
            foreach (var service in services.Where(s => s.Name.StartsWith(TestServiceNamePrefix)))
            {
                foreach (var route in service.Routes)
                {
                    await _kongAdminWriter.DeleteRoute(route.Id);
                }

                await _kongAdminWriter.DeleteService(service.Id);
            }

            return true;
        }

        public async Task<KongService> AddServiceAndChildren(KongService service)
        {
            var addedService = await _kongAdminWriter.AddService(service);
            addedService.Should().NotBeNull();
            addedService.Id.Should().NotBeNullOrEmpty();

            if (service.Plugins != null)
            {
                foreach (var plugin in service.Plugins)
                {
                    var pluginBody = PluginCollection.CreatePluginBody(plugin);
                    pluginBody.service_id = service.Id;

                    await UpsertPlugin(pluginBody);
                }
            }

            if (service.Routes != null)
            {
                foreach (var route in service.Routes)
                {
                    var addedRoute = await _kongAdminWriter.AddRoute(service, route);
                    addedRoute.Should().NotBeNull();

                    foreach (var plugin in route.Plugins)
                    {
                        var pluginBody = PluginCollection.CreatePluginBody(plugin);
                        pluginBody.service_id = service.Id;
                        pluginBody.route_id = route.Id;

                        await UpsertPlugin(pluginBody);
                    }
                }
            }

            _cleanUp.Add(service);
            return addedService;
        }

        public async Task DeleteServiceWithChildren(KongService service)
        {
            await DeleteServiceWithChildrenInternal(service);
            _cleanUp.Remove(service);
        }

        private async Task DeleteServiceWithChildrenInternal(KongService service)
        {
            await _kongAdminWriter.DeleteRoutes(service);
            await _kongAdminWriter.DeleteService(service.Id);
        }

        public async Task DeleteGlobalPlugin(string id)
        {
            await _kongAdminWriter.DeletePlugin(id);
        }

        public async Task<KongPluginResponse> UpsertPlugin(PluginBody pluginBody)
        {
            var plugin = await _kongAdminWriter.UpsertPlugin(pluginBody);
            plugin.Should().NotBeNull();
            plugin.Id.Should().NotBeNullOrEmpty();

            return plugin;
        }

        public void Dispose()
        {
            foreach (var service in _cleanUp)
            {
                DeleteServiceWithChildrenInternal(service).Wait();
            }
        }
    }
}
