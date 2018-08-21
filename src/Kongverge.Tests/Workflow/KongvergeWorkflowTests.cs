using System;
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
using Kongverge.KongPlugin;
using KongVerge.Tests.Serialization;
using Kongverge.Common;

namespace KongVerge.Tests.Workflow
{
    public class KongvergeWorkflowTests
    {
        private readonly Fixture _fixture = new Fixture();

        public class KongvergeWorkflowSut
        {
            private static readonly Fixture Fixture = new Fixture();

            public Mock<IDataFileHelper> DataFiles = new Mock<IDataFileHelper>();
            public Mock<IKongAdminReader> KongReader = new Mock<IKongAdminReader>();
            public Mock<IKongAdminWriter> KongWriter = new Mock<IKongAdminWriter>();
            public Mock<IKongPluginCollection> KongPluginCollection = new Mock<IKongPluginCollection>();

            public Settings Settings { get; }
            public KongvergeWorkflow Sut { get; }

            public KongvergeWorkflowSut()
            {
                KongReader.Setup(k => k.KongIsReachable()).ReturnsAsync(true);

                Settings = new Settings
                {
                    Admin = Fixture.Create<Admin>()
                };

                var configuration = new Mock<IOptions<Settings>>();
                configuration.Setup(c => c.Value).Returns(Settings);

                Sut = new KongvergeWorkflow(
                    KongReader.Object,
                    configuration.Object,
                    KongWriter.Object,
                    DataFiles.Object,
                    KongPluginCollection.Object);
            }
        }

        [Fact]
        public async Task ConvergeRoutes_WillAddMissingRoutes()
        {
            var system = new KongvergeWorkflowSut();
            var route1 = new KongRoute();

            var service = new KongService
            {
                Routes = new List<KongRoute>
                {
                    route1
                }
            };

            await system.Sut.ConvergeRoutes(service, Array.Empty<KongRoute>());

            system.KongWriter.Verify(k => k.AddRoute(service, route1), Times.Once());
        }

        [Fact]
        public async Task ConvergeRoutes_WillRemoveExcessRoutes()
        {
            var system = new KongvergeWorkflowSut();
            var route1 = new KongRoute
            {
                Id = Guid.NewGuid().ToString()
            };

            var service = new KongService
            {
                Routes = new List<KongRoute>()
            };

            var routes = new List<KongRoute>
                {
                    route1
                };

            await system.Sut.ConvergeRoutes(service, routes);

            system.KongWriter.Verify(k => k.DeleteRoute(route1.Id), Times.Once());
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

            var system = new KongvergeWorkflowSut();

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

            var system = new KongvergeWorkflowSut();

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

            var system = new KongvergeWorkflowSut();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            await system.Sut.ConvergePlugins(targetService, service);

            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());
        }

        [Fact]
        public async Task GlobalConfig_NoChangesIfMatch()
        {
            var plugin = _fixture.Create<TestKongConfig>();

            var clusterConfig = new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>
                {
                    plugin
                }
            };

            var fileConfig = new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>
                {
                    plugin
                }
            };

            var system = SetupExecute_WithNoServiceChanges(clusterConfig, fileConfig);

            await system.Sut.DoExecute();

            system.KongWriter.Verify(kong => kong.UpsertPlugin(It.IsAny<PluginBody>()), Times.Never());
        }

        [Fact]
        public async Task GlobalConfig_AddsPluginGlobally()
        {
            var plugin = _fixture.Create<TestKongConfig>();

            var clusterConfig = new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>()
            };

            var fileConfig = new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>
                {
                    plugin
                }
            };

            var body = _fixture.Create<PluginBody>();

            var system = SetupExecute_WithNoServiceChanges(clusterConfig, fileConfig);

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin)).Returns(body);

            await system.Sut.DoExecute();

            system.KongWriter.Verify(kong => kong.UpsertPlugin(It.IsAny<PluginBody>()), Times.Once());
        }

        [Fact]
        public async Task GlobalConfig_UpdatesPluginGlobally()
        {
            var plugin = _fixture.Create<TestKongConfig>();
            var plugin2 = _fixture.Create<TestKongConfig>();

            var clusterConfig = new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>
                {
                    plugin2
                }
            };

            var fileConfig = new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>
                {
                    plugin
                }
            };

            var body = _fixture.Create<PluginBody>();

            var system = SetupExecute_WithNoServiceChanges(clusterConfig, fileConfig);

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin)).Returns(body);

            await system.Sut.DoExecute();

            system.KongWriter.Verify(kong => kong.UpsertPlugin(It.IsAny<PluginBody>()), Times.Once());
        }

        [Fact]
        public async Task GlobalConfig_RemovesPluginGlobally()
        {
            var plugin = _fixture.Create<TestKongConfig>();

            var clusterConfig = new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>
                {
                    plugin
                }
            };

            var fileConfig = new GlobalConfig
            {
                Plugins = new List<IKongPluginConfig>()
            };

            var body = _fixture.Create<PluginBody>();

            var system = SetupExecute_WithNoServiceChanges(clusterConfig, fileConfig);

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin)).Returns(body);

            await system.Sut.DoExecute();

            system.KongWriter.Verify(kong => kong.DeletePlugin(plugin.id), Times.Once());
        }

        private static KongvergeWorkflowSut SetupExecute_WithNoServiceChanges(GlobalConfig clusterConfig, GlobalConfig fileConfig)
        {
            IReadOnlyCollection<KongDataFile> files = Array.Empty<KongDataFile>();

            var system = new KongvergeWorkflowSut();

            system.KongReader
                  .Setup(kong => kong.GetServices())
                  .ReturnsAsync(new List<KongService>());

            system.KongReader
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
                  .Returns(true)
                  .Verifiable();
            return system;
        }
    }
}