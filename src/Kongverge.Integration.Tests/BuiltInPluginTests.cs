using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class BuiltInPluginTests : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;

        public BuiltInPluginTests(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
        }

        [Fact]
        public async Task DefaultServiceHasNoPlugins()
        {
            var service = new ServiceBuilder().AddDefaultTestService().Build();
            var kongServiceAdded = await _fixture.AddServiceAndPlugins(service);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            serviceReadFromKong.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().BeEmpty();
        }

        [Fact]
        public async Task ServiceCanHaveRateLimitingPlugin()
        {
            var plugin = new RateLimitingConfig
            {
                Identifier = "consumer",
                Limit = new [] { 123 },
                WindowSize = new [] { 3455 }
            };

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<RateLimitingConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveCorrelationIdPlugin()
        {
            var plugin = new CorrelationIdConfig
            {
                Header = "test1"
            };

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<CorrelationIdConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveKeyAuthenticationPlugin()
        {
            var plugin = new KeyAuthenticationConfig();

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<KeyAuthenticationConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTerminationPlugin()
        {
            var plugin = new RequestTerminationConfig
            {
                StatusCode = 501,
                Message = "test term"
            };

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<RequestTerminationConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }
    }
}
