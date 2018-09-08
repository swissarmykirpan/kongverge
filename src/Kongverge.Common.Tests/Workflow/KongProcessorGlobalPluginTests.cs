using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
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
            var plugin = fixture.Create<KongPlugin>();
            var clusterConfig = GlobalPluginConfig(plugin);
            var fileConfig = GlobalPluginConfig(plugin);

            await _system.Processor.Process(_noServices, _noServices, clusterConfig, fileConfig);

            _system.KongWriter.Verify(kong => kong.UpsertPlugin(It.IsAny<KongPlugin>()), Times.Never());
        }

        [Fact]
        public async Task CanAddGlobalPlugin()
        {
            var fixture = new Fixture();
            var plugin = fixture.Create<KongPlugin>();

            var clusterConfig = EmptyGlobalConfig();
            var fileConfig = GlobalPluginConfig(plugin);

            await _system.Processor.Process(_noServices, _noServices, clusterConfig, fileConfig);

            _system.KongWriter.Verify(kong => kong.UpsertPlugin(plugin), Times.Once());
        }

        [Fact]
        public async Task CanUpdateGlobalPlugin()
        {
            var fixture = new Fixture();
            var plugin = fixture.Create<KongPlugin>();
            var plugin2 = fixture.Create<KongPlugin>();

            var clusterConfig = GlobalPluginConfig(plugin2);
            var fileConfig = GlobalPluginConfig(plugin);

            await _system.Processor.Process(_noServices, _noServices, clusterConfig, fileConfig);

            _system.KongWriter.Verify(kong => kong.UpsertPlugin(plugin), Times.Once());
        }

        [Fact]
        public async Task CanDeleteGlobalPlugin()
        {
            var fixture = new Fixture();
            var plugin = fixture.Create<KongPlugin>();

            var clusterConfig = GlobalPluginConfig(plugin);
            var fileConfig = EmptyGlobalConfig();

            await _system.Processor.Process(_noServices, _noServices, clusterConfig, fileConfig);

            _system.KongWriter.Verify(kong => kong.DeletePlugin(plugin.Id), Times.Once());
        }

        private static ExtendibleKongObject GlobalPluginConfig(KongPlugin plugin)
        {
            return new ExtendibleKongObject
            {
                Plugins = new[]
                {
                    plugin
                }
            };
        }

        private static ExtendibleKongObject EmptyGlobalConfig()
        {
            return new ExtendibleKongObject();
        }
    }
}
