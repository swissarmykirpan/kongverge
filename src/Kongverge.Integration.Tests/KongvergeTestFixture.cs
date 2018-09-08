using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kongverge.Integration.Tests
{
    public class KongvergeTestFixture : IDisposable
    {
        public const string Host = "kongtestbed-qa8-alb.jalfrezi.je-labs.com";
        public const int Port = 8001;
        public const string TestServiceNamePrefix = "testservice_";

        private readonly ServiceProvider _serviceProvider;
        private readonly IList<KongService> _cleanUp;
        private readonly IKongAdminWriter _kongAdminWriter;

        public IKongAdminReader KongAdminReader => _serviceProvider.GetService<IKongAdminReader>();
        public ServiceBuilder ServiceBuilder = new ServiceBuilder(TestServiceNamePrefix);

        public KongvergeTestFixture()
        {
            var services = new ServiceCollection();
            ServiceRegistration.AddServices(services);
            _serviceProvider = services.BuildServiceProvider();
            ConfigureSettings();
            _kongAdminWriter = _serviceProvider.GetService<IKongAdminWriter>();
            _cleanUp = new List<KongService>();
            DeleteExistingKongObjects();
        }

        private void ConfigureSettings()
        {
            var configuration = _serviceProvider.GetService<IOptions<Settings>>().Value;
            configuration.Admin.Host = Host;
            configuration.Admin.Port = Port;
        }

        private void DeleteExistingKongObjects()
        {
            DeleteExistingTestServices().GetAwaiter().GetResult();
            DeleteExistingGlobalPlugins().GetAwaiter().GetResult();
        }

        private async Task<bool> DeleteExistingGlobalPlugins()
        {
            var plugins = await KongAdminReader.GetPlugins();
            var globalPlugins = plugins.Where(x => x.IsGlobal());
            foreach (var plugin in globalPlugins)
            {
                await _kongAdminWriter.DeletePlugin(plugin.Id);
            }

            return true;
        }

        private async Task<bool> DeleteExistingTestServices()
        {
            var services = await KongAdminReader.GetServices();
            var routes = await KongAdminReader.GetRoutes();
            foreach (var route in routes.Where(x => services.Select(s => s.Id).Contains(x.Service.Id)))
            {
                await _kongAdminWriter.DeleteRoute(route.Id);
            }
            foreach (var service in services.Where(s => s.Name.StartsWith(TestServiceNamePrefix)))
            {
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
                    service.AssignParentId(plugin);
                    await UpsertPlugin(plugin);
                }
            }

            if (service.Routes != null)
            {
                foreach (var route in service.Routes)
                {
                    await _kongAdminWriter.AddRoute(service.Id, route);
                    route.Id.Should().NotBeNullOrEmpty();

                    foreach (var plugin in route.Plugins)
                    {
                        route.AssignParentId(plugin);
                        await UpsertPlugin(plugin);
                    }
                }
            }

            _cleanUp.Add(service);
        }

        public async Task DeleteServiceWithChildren(KongService service)
        {
            await _kongAdminWriter.DeleteService(service.Id);
            _cleanUp.Remove(service);
        }

        public async Task DeleteGlobalPlugin(string id)
        {
            await _kongAdminWriter.DeletePlugin(id);
        }

        public async Task UpsertPlugin(KongPlugin plugin)
        {
            await _kongAdminWriter.UpsertPlugin(plugin);
            plugin.Id.Should().NotBeNullOrEmpty();
        }

        public void Dispose()
        {
            foreach (var service in _cleanUp)
            {
                _kongAdminWriter.DeleteService(service.Id).Wait();
            }
        }
    }
}
