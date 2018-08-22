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
        private readonly ServiceProvider _serviceProvider;
        public IDataFileHelper DataFileHelper => _serviceProvider.GetService<IDataFileHelper>();
        public IKongAdminWriter KongAdminWriter => _serviceProvider.GetService<IKongAdminWriter>();
        public IKongAdminReader KongAdminReader => _serviceProvider.GetService<IKongAdminReader>();
        public Settings Settings => _serviceProvider.GetRequiredService<IOptions<Settings>>().Value;
        public IList<KongService> CleanUp { get; }
        public IKongPluginCollection PluginCollection => new KongPluginCollection(_serviceProvider.GetServices<IKongPlugin>());

        public KongvergeTestFixture()
        {
            var services = new ServiceCollection();
            ServiceRegistration.AddServices(services);
            _serviceProvider = services.BuildServiceProvider();
            CleanUp = new List<KongService>();
            DeleteExistingTestServices().GetAwaiter().GetResult();
        }

        private async Task<bool> DeleteExistingTestServices()
        {
            var services = await KongAdminReader.GetServices();
            foreach (var service in services.Where(s => s.Name.StartsWith("testservice_")))
            {
                foreach (var route in service.Routes)
                {
                    await KongAdminWriter.DeleteRoute(route.Id);
                }

                await KongAdminWriter.DeleteService(service.Id);
            }

            return true;
        }

        public async Task<KongService> AddServiceAndChildren(KongService service)
        {
            var addedService = await KongAdminWriter.AddService(service);
            addedService.Should().NotBeNull();
            addedService.Id.Should().NotBeNullOrEmpty();

            if (service.Plugins != null)
            {
                foreach (var plugin in service.Plugins)
                {
                    var pluginBody = PluginCollection.CreatePluginBody(plugin);
                    pluginBody.service_id = service.Id;

                    await ShouldUpsertPlugin(pluginBody);
                }
            }

            if (service.Routes != null)
            {
                foreach (var route in service.Routes)
                {
                    var addedRoute = await KongAdminWriter.AddRoute(service, route);
                    addedRoute.Should().NotBeNull();

                    foreach (var plugin in route.Plugins)
                    {
                        var pluginBody = PluginCollection.CreatePluginBody(plugin);
                        pluginBody.service_id = service.Id;
                        pluginBody.route_id = route.Id;

                        await ShouldUpsertPlugin(pluginBody);
                    }
                }
            }

            CleanUp.Add(service);
            return addedService;
        }

        private async Task<KongPluginResponse> ShouldUpsertPlugin(PluginBody pluginBody)
        {
            var plugin = await KongAdminWriter.UpsertPlugin(pluginBody);
            plugin.Should().NotBeNull();
            plugin.Id.Should().NotBeNullOrEmpty();

            return plugin;
        }

        public void Dispose()
        {
            foreach (var service in CleanUp)
            {
                KongAdminWriter.DeleteRoutes(service).Wait();
                KongAdminWriter.DeleteService(service.Id).Wait();
            }
        }
    }
}
