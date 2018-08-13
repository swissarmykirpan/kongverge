using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.Plugins.Custom;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class CustomPluginTests : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;

        public CustomPluginTests(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
        }

        [Fact]
        public async Task ServiceCanHaveJeJustSayingDefaultsPlugin()
        {
            var plugin = new JeJustSayingDefaultsConfig
            {
                RaisingComponent = "testFeature",
                Tenant = "BW"
            };

            var kongAction = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongAction.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<JeJustSayingDefaultsConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }
    }
}
