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


        [Fact]
        public async Task ServiceCanHaveJeRequestDelayPlugin()
        {
            var plugin = new JeRequestDelayConfig
            {
                Delay = 123
            };

            var kongAction = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongAction.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<JeRequestDelayConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveJeTcpLogPlugin()
        {
            var plugin = new JeTcpLogConfig
            {
                Host = "testHost",
                KeepAlive = 3,
                LogBody = true,
                MaxBodySize = 1234,
                Port = 8125,
                Timeout = 3456
            };

            var kongAction = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongAction.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<JeTcpLogConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveJeUdpLogPlugin()
        {
            var plugin = new JeUdpLogConfig
            {
                Host = "testHost",
                LogBody = true,
                MaxBodySize = 1234,
                Port = 8125,
                Timeout = 3456
            };

            var kongAction = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongAction.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<JeUdpLogConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }
    }
}
