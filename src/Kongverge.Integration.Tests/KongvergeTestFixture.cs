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

        public async Task AddServiceAndChildren(KongService service)
        {
            await _kongAdminWriter.AddService(service);
            service.Id.Should().NotBeNullOrEmpty();

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
                    await _kongAdminWriter.AddRoute(service, route);
                    route.Id.Should().NotBeNullOrEmpty();

                    foreach (var plugin in route.Plugins)
                    {
                        var pluginBody = PluginCollection.CreatePluginBody(plugin);
                        pluginBody.service_id = service.Id;
                        pluginBody.route_id = route.Id;

                        await ShouldUpsertPlugin(pluginBody);
                    }
                }
            }

            _cleanUp.Add(service);
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

        private async Task ShouldUpsertPlugin(PluginBody pluginBody)
        {
            await _kongAdminWriter.UpsertPlugin(pluginBody);
            pluginBody.id.Should().NotBeNullOrEmpty();
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
