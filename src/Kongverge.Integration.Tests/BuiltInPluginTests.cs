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
        public async Task ServiceCanHaveCorrelationIdPlugin()
        {
            var plugin = new CorrelationIdConfig
            {
                HeaderName = "test1"
            };

            await _fixture.ShouldRoundTripPluginToKong(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveKeyAuthenticationPlugin()
        {
            var plugin = new KeyAuthenticationConfig();

            await _fixture.ShouldRoundTripPluginToKong(plugin);
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

            await _fixture.ShouldRoundTripPluginToKong(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTerminationPlugin()
        {
            var plugin = new RequestTerminationConfig
            {
                StatusCode = 501,
                Message = "test term"
            };

            await _fixture.ShouldRoundTripPluginToKong(plugin);
        }
    }
}
