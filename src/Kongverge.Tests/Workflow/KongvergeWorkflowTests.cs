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
