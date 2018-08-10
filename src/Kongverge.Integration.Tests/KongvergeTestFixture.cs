using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            DeleteExistingTestServices().Wait();
        }

        private async Task DeleteExistingTestServices()
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

        }

        public void Dispose()
        {
            foreach (var service in CleanUp)
            {
                KongAdminWriter.DeleteService(service.Id).Wait();
            }
        }
    }
}
