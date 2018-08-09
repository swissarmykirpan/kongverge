using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.Common.Plugins.BuiltIn;
using Kongverge.Common.Services;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class PluginTests : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;

        public PluginTests(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
        }

        [Fact]
        public async Task DefaultServiceHasNoPlugins()
        {
            var service = new ServiceBuilder().AddDefaultTestService().Build();
            var kongAction = await AddServiceAndPlugins(service);
            _fixture.CleanUp.Add(service);
            kongAction.Succeeded.Should().BeTrue();
            kongAction.Result.Id.Should().NotBeNullOrEmpty();

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongAction.Result.Id);

            serviceReadFromKong.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().BeEmpty();
        }

        [Fact]
        public async Task ServiceCanHaveRateLimitingPlugin()
        {
            var plugin = new RateLimitingConfig
            {
                Limit = new [] { 123 },
                WindowSize = new [] { 3455 }
            };

            var service = new ServiceBuilder()
                .AddDefaultTestService()
                .WithPlugin(plugin)
                .Build();

            var kongAction = await AddServiceAndPlugins(service);
            _fixture.CleanUp.Add(service);
            kongAction.Succeeded.Should().BeTrue();
            kongAction.Result.Id.Should().NotBeNullOrEmpty();

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongAction.Result.Id);

            serviceReadFromKong.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().HaveCount(1);
        }

        private async Task<KongAction<KongService>> AddServiceAndPlugins(KongService service)
        {
            var kongAction = await _fixture.KongAdminWriter.AddService(service);
            kongAction.Succeeded.Should().BeTrue();

            if (service.Plugins != null)
            {
                foreach (var plugin in service.Plugins)
                {
                    var content = _fixture.PluginCollection.CreatePluginBody(plugin);
                    content.service_id = service.Id;
                    var pluginResult = await _fixture.KongAdminWriter.UpsertPlugin(content);
                    pluginResult.Should().NotBeNull();
                }
            }

            return kongAction;
        }
    }
}
