using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
using Moq;
using Xunit;

namespace Kongverge.Common.Tests.Workflow
{
    public class KongProcessorGlobalPluginTests : KongProcessorTestsBase
    {
        private readonly IReadOnlyCollection<KongService> _noServices = new List<KongService>();

        [Fact]
        public async Task NoChangesIfGlobalPluginsMatch()
        {
            var plugin = this.Create<KongPlugin>();
            var existingGlobalConfig = GlobalPluginConfig(plugin);
            var targetGlobalConfig = GlobalPluginConfig(plugin);

            await Processor.Process(_noServices, _noServices, existingGlobalConfig, targetGlobalConfig);

            KongWriter.Verify(x => x.UpsertPlugin(It.IsAny<KongPlugin>()), Times.Never());
        }

        [Fact]
        public async Task CanAddGlobalPlugin()
        {
            var target = this.Create<KongPlugin>();

            var existingGlobalConfig = EmptyGlobalConfig();
            var targetGlobalConfig = GlobalPluginConfig(target);

            await Processor.Process(_noServices, _noServices, existingGlobalConfig, targetGlobalConfig);

            KongWriter.Verify(x => x.UpsertPlugin(target), Times.Once());
        }

        [Fact]
        public async Task CanUpdateGlobalPlugin()
        {
            var target = this.Create<KongPlugin>();
            var existing = this.Create<KongPlugin>();

            var existingGlobalConfig = GlobalPluginConfig(existing);
            var targetGlobalConfig = GlobalPluginConfig(target);

            await Processor.Process(_noServices, _noServices, existingGlobalConfig, targetGlobalConfig);

            KongWriter.Verify(x => x.UpsertPlugin(target), Times.Once());
        }

        [Fact]
        public async Task CanDeleteGlobalPlugin()
        {
            var existing = this.Create<KongPlugin>();

            var existingGlobalConfig = GlobalPluginConfig(existing);
            var targetGlobalConfig = EmptyGlobalConfig();

            await Processor.Process(_noServices, _noServices, existingGlobalConfig, targetGlobalConfig);

            KongWriter.Verify(x => x.DeletePlugin(existing.Id), Times.Once());
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
