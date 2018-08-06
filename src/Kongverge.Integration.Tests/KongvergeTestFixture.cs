using System;
using System.Collections.Generic;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kongverge.Integration.Tests
{
    public class KongvergeTestFixture : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        public IDataFileHelper DataFileHelper => _serviceProvider.GetService<IDataFileHelper>();
        public IKongAdminWriteService KongAdminService => _serviceProvider.GetService<IKongAdminWriteService>();
        public Settings Settings => _serviceProvider.GetRequiredService<IOptions<Settings>>().Value;

        public IList<KongService> CleanUp { get; }

        public KongvergeTestFixture()
        {
            var services = new ServiceCollection();
            ServiceRegistration.AddServices(services);
            _serviceProvider = services.BuildServiceProvider();
            CleanUp = new List<KongService>();
        }

        public void Dispose()
        {
            foreach (var service in CleanUp)
            {
                KongAdminService.DeleteService(service).Wait();
            }
        }
    }
}
