using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;

namespace Kongverge.Integration.Tests
{
    public static class PluginTestHelpers
    {
        public static async Task<KongService> AttachPluginToService(this KongvergeTestFixture fixture, IKongPluginConfig plugin)
        {
            var service = new ServiceBuilder()
                .AddDefaultTestService()
                .WithPlugin(plugin)
                .Build();

            return await AddServiceAndPlugins(fixture, service);
        }

        public static async Task<KongService> AddServiceAndPlugins(this KongvergeTestFixture fixture, KongService service)
        {
            var addServiceResult = await fixture.KongAdminWriter.AddService(service);
            addServiceResult.Should().NotBeNull();
            addServiceResult.ShouldSucceed();
            addServiceResult.Result.Id.Should().NotBeNullOrEmpty();

            if (service.Plugins != null)
            {
                foreach (var plugin in service.Plugins)
                {
                    var content = fixture.PluginCollection.CreatePluginBody(plugin);
                    content.service_id = service.Id;
                    var pluginResult = await fixture.KongAdminWriter.UpsertPlugin(content);
                    pluginResult.Should().NotBeNull();
                    pluginResult.ShouldSucceed();
                    pluginResult.Result.Id.Should().NotBeNullOrEmpty();
                }
            }

            fixture.CleanUp.Add(service);
            return addServiceResult.Result;
        }

        public static T ShouldHaveOnePlugin<T>(this KongService service) where T : IKongPluginConfig
        {
            service.Should().NotBeNull();
            service.Plugins.Should().NotBeNull();
            service.Plugins.Should().HaveCount(1);
            service.Plugins[0].Should().BeOfType<T>();

            return (T)service.Plugins[0];
        }
    }
}
