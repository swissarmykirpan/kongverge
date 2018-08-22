using Kongverge.Common.DTOs;
using Kongverge.Common.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using Kongverge.Common.Plugins;
using Kongverge.KongPlugin;
using KongVerge.Tests.Serialization;
using Kongverge.Common;

namespace KongVerge.Tests.Workflow
{
    public class ServicesProcessorTests
    {
        private readonly Fixture _fixture = new Fixture();

        public class ServicesProcessorSut
        {
            public Mock<IKongAdminWriter> KongWriter = new Mock<IKongAdminWriter>();
            public Mock<IKongPluginCollection> KongPluginCollection = new Mock<IKongPluginCollection>();

            public ServicesProcessor Sut { get; }

            public ServicesProcessorSut()
            {
                Sut = new ServicesProcessor(
                    KongWriter.Object,
                    KongPluginCollection.Object);
            }
        }

        [Fact]
        public async Task ConvergePlugins_WillRemoveExcessPlugins()
        {
            var plugin2 = _fixture.Create<TestKongConfig>();
            var plugin1 = _fixture.Create<OtherTestKongConfig>();

            var service = new KongService
            {
                Plugins = new List<IKongPluginConfig>
                { plugin1, plugin2 }
            };

            var targetService = new KongService();

            var system = new ServicesProcessorSut();

            await system.Sut.ConvergePlugins(targetService, service);

            system.KongWriter.Verify(k => k.DeletePlugin(plugin1.id), Times.Once());
            system.KongWriter.Verify(k => k.DeletePlugin(plugin2.id), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillAddMissingPlugins()
        {
            var plugin2 = _fixture.Create<TestKongConfig>();
            var plugin1 = _fixture.Create<OtherTestKongConfig>();
            var body1 = _fixture.Create<PluginBody>();
            var body2 = _fixture.Create<PluginBody>();

            var service = new KongService();

            var targetService = new KongService
            {
                Plugins = new List<IKongPluginConfig>
                { plugin1, plugin2 }
            };

            var system = new ServicesProcessorSut();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);
            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin2)).Returns(body2);

            await system.Sut.ConvergePlugins(targetService, service);

            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());
            system.KongWriter.Verify(k => k.UpsertPlugin(body2), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillUpdateChangedPlugins()
        {
            var plugin2 = _fixture.Create<TestKongConfig>();
            var plugin1 = _fixture.Create<TestKongConfig>();
            var body1 = _fixture.Create<PluginBody>();

            var service = new KongService
            {
                Plugins = new List<IKongPluginConfig> { plugin2 }
            };

            var targetService = new KongService
            {
                Plugins = new List<IKongPluginConfig> { plugin1 }
            };

            var system = new ServicesProcessorSut();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            await system.Sut.ConvergePlugins(targetService, service);

            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());
        }
    }
}
