using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
using Kongverge.Common.Tests.Helpers;
using Kongverge.KongPlugin;
using Moq;
using Xunit;

namespace Kongverge.Common.Tests.Workflow
{
    public class KongProcessorGlobalPluginTests
    {
        private readonly IReadOnlyCollection<KongService> _noServices = new List<KongService>();
        private readonly KongProcessorEnvironment _system = new KongProcessorEnvironment();

        [Fact]
        public async Task NoChangesIfGlobalPluginsMatch()
        {
            var fixture = new Fixture();
            var plugin = fixture.Create<TestKongConfig>();
            var clusterConfig = GlobalPluginConfig(plugin);
            var fileConfig = GlobalPluginConfig(plugin);

            await _system.Processor.Process(_noServices, _noServices, clusterConfig, fileConfig);

            _system.KongWriter.Verify(kong => kong.UpsertPlugin(It.IsAny<PluginBody>()), Times.Never());
        }

        [Fact]
        public async Task CanAddGlobalPlugin()
        {
            var fixture = new Fixture();
            var plugin = fixture.Create<TestKongConfig>();

            var clusterConfig = EmptyGlobalConfig();
            var fileConfig = GlobalPluginConfig(plugin);

            var pluginBody = fixture.Create<PluginBody>();
            _system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin))
                   .Returns(pluginBody);

            await _system.Processor.Process(_noServices, _noServices, clusterConfig, fileConfig);

            _system.KongWriter.Verify(kong => kong.UpsertPlugin(pluginBody), Times.Once());
        }

        [Fact]
        public async Task CanUpdateGlobalPlugin()
        {
            var fixture = new Fixture();
            var plugin = fixture.Create<TestKongConfig>();
            var plugin2 = fixture.Create<TestKongConfig>();

            var clusterConfig = GlobalPluginConfig(plugin2);
            var fileConfig = GlobalPluginConfig(plugin);

            var pluginBody = fixture.Create<PluginBody>();
            _system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin))
                   .Returns(pluginBody);

            await _system.Processor.Process(_noServices, _noServices, clusterConfig, fileConfig);

            _system.KongWriter.Verify(kong => kong.UpsertPlugin(pluginBody), Times.Once());
        }

        [Fact]
        public async Task CanDeleteGlobalPlugin()
        {
            var fixture = new Fixture();
            var plugin = fixture.Create<TestKongConfig>();

            var clusterConfig = GlobalPluginConfig(plugin);
            var fileConfig = EmptyGlobalConfig();

            _system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin))
                   .Returns(fixture.Create<PluginBody>());

            await _system.Processor.Process(_noServices, _noServices, clusterConfig, fileConfig);

            _system.KongWriter.Verify(kong => kong.DeletePlugin(plugin.id), Times.Once());
        }

        private static GlobalConfig GlobalPluginConfig(IKongPluginConfig plugin)
        {
            return new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>
                {
                    plugin
                }
            };
        }

        private static GlobalConfig EmptyGlobalConfig()
        {
            return new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>()
            };
        }
    }
}
