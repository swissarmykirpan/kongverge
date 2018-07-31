using Kongverge;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using Kongverge.Common.Plugins;
using System.Linq;
using Kongverge.KongPlugin;
using KongVerge.Tests.Serialization;

namespace KongVerge.Tests.Workflow
{
    public class KongvergeWorkflowTests
    {
        private readonly Fixture _fixture = new Fixture();

        public class KongvergeWorkflowSut
        {
            private static Fixture _fixture = new Fixture();

            public Mock<IDataFileHelper> DataFiles = new Mock<IDataFileHelper>();
            public Mock<IKongAdminService> KongService = new Mock<IKongAdminService>();
            public Mock<IKongPluginCollection> KongPluginCollection = new Mock<IKongPluginCollection>();

            public Settings Settings { get; }
            public KongvergeWorkflow Sut { get; }

            public KongvergeWorkflowSut()
            {
                KongService.Setup(k => k.KongIsReachable()).ReturnsAsync(true);

                Settings = new Settings()
                {
                    Admin = _fixture.Create<Admin>()
                };

                var configuration = new Mock<IOptions<Settings>>();
                configuration.Setup(c => c.Value).Returns(Settings);

                Sut = new KongvergeWorkflow(
                    KongService.Object,
                    DataFiles.
                    Object, configuration.Object,
                    KongPluginCollection.Object);
            }
        }

        [Fact]
        public async Task ConvergeRoutes_WillAddMissingRoutes()
        {
            var system = new KongvergeWorkflowSut();
            var route1 = new KongRoute();

            var service = new KongService()
            {
                Routes = new List<KongRoute>()
                {
                    route1
                }
            };

            await system.Sut.ConvergeRoutes(service, Enumerable.Empty<KongRoute>());

            system.KongService.Verify(k => k.AddRoute(service, route1), Times.Once());
        }

        [Fact]
        public async Task ConvergeRoutes_WillRemoveExcessRoutes()
        {
            var system = new KongvergeWorkflowSut();
            var route1 = new KongRoute();

            var service = new KongService()
            {
                Routes = new List<KongRoute>()
            };

            var routes = new List<KongRoute>
                {
                    route1
                };

            await system.Sut.ConvergeRoutes(service, routes);

            system.KongService.Verify(k => k.DeleteRoute(route1), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillRemoveExcessPlugins()
        {
            var plugin2 = _fixture.Create<TestKongConfig>();
            var plugin1 = _fixture.Create<OtherTestKongConfig>();

            var service = new KongService()
            {
                Extensions = new List<IKongPluginConfig>
                { plugin1, plugin2 }
            };

            var targetService = new KongService()
            {
            };

            var system = new KongvergeWorkflowSut();

            await system.Sut.ConvergePlugins(targetService, service);

            system.KongService.Verify(k => k.DeletePlugin(plugin1.id), Times.Once());
            system.KongService.Verify(k => k.DeletePlugin(plugin2.id), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillAddMissingPlugins()
        {
            var plugin2 = _fixture.Create<TestKongConfig>();
            var plugin1 = _fixture.Create<OtherTestKongConfig>();
            var body1 = _fixture.Create<PluginBody>();
            var body2 = _fixture.Create<PluginBody>();

            var service = new KongService();

            var targetService = new KongService()
            {
                Extensions = new List<IKongPluginConfig>
                { plugin1, plugin2 }
            };

            var system = new KongvergeWorkflowSut();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);
            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin2)).Returns(body2);

            await system.Sut.ConvergePlugins(targetService, service);

            system.KongService.Verify(k => k.UpsertPlugin(body1), Times.Once());
            system.KongService.Verify(k => k.UpsertPlugin(body2), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillUpdateChangedPlugins()
        {
            var plugin2 = _fixture.Create<TestKongConfig>();
            var plugin1 = _fixture.Create<TestKongConfig>();
            var body1 = _fixture.Create<PluginBody>();

            var service = new KongService()
            {
                Extensions = new List<IKongPluginConfig> { plugin2 }
            };

            var targetService = new KongService()
            {
                Extensions = new List<IKongPluginConfig> { plugin1 }
            };

            var system = new KongvergeWorkflowSut();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            await system.Sut.ConvergePlugins(targetService, service);

            system.KongService.Verify(k => k.UpsertPlugin(body1), Times.Once());
        }

        [Fact]
        public async Task GlobalConfigRetrieved_AndConverged()
        {
            var clusterConfig = _fixture.Create<GlobalConfig>();
            var fileConfig = _fixture.Create<GlobalConfig>();
            var files = _fixture.Create<List<KongDataFile>>();

            var system = new KongvergeWorkflowSut();

            system.KongService
                  .Setup(kong =>
                            kong.GetGlobalConfig())
                  .ReturnsAsync(KongAction.Success(clusterConfig))
                  .Verifiable();

            system.DataFiles
                  .Setup(file =>
                            file.GetDataFiles(
                                It.IsAny<string>(),
                                out files,
                                out fileConfig))
                  .Verifiable();

            await system.Sut.DoExecute();

            system.KongService.VerifyAll();
            system.DataFiles.VerifyAll();
        }
    }
}
