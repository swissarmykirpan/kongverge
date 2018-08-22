using System;
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
        public async Task NoOpWorks()
        {
            var system = new ServicesProcessorSut();

            var existingServices = new List<KongService>();
            var newServices = new List<KongService>();

            await system.Sut.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k =>
                k.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Never);
        }

        [Fact]
        public async Task ConvergeRoutes_WillAddMissingRoutes()
        {
            var system = new ServicesProcessorSut();
            var route1 = new KongRoute();

            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService1"
                }
            };

            var newServices = new List<KongService>
            { 
                new KongService
                {
                    Name = "TestService1",
                    Routes = new List<KongRoute>
                    {
                        route1
                    }
                }
            };

           await system.Sut.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k => k.AddRoute(newServices[0], route1), Times.Once());
        }

        [Fact]
        public async Task ConvergeRoutes_WillRemoveExcessRoutes()
        {
            var system = new ServicesProcessorSut();
            var route1 = new KongRoute
            {
                Id = Guid.NewGuid().ToString()
            };

            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService2",
                    Routes = new List<KongRoute>
                    {
                        route1
                    }
                }
            };

            var newServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService2",
                    Routes = new List<KongRoute>()
                }
            };

            await system.Sut.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k => k.DeleteRoute(route1.Id), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillRemoveExcessPlugins()
        {
            var plugin2 = _fixture.Create<TestKongConfig>();
            var plugin1 = _fixture.Create<OtherTestKongConfig>();

            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService3",
                    Plugins = new List<IKongPluginConfig>
                    {
                        plugin1,
                        plugin2
                    }
                }
            };

            var newServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService3"
                }
            };

            var system = new ServicesProcessorSut();

            await system.Sut.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

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

            var system = new ServicesProcessorSut();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);
            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin2)).Returns(body2);

            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService4"
                }
            };

            var newServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService4",
                    Plugins = new List<IKongPluginConfig>
                    {
                        plugin1,
                        plugin2
                    }
                }
            };

            await system.Sut.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());
            system.KongWriter.Verify(k => k.UpsertPlugin(body2), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillUpdateChangedPlugins()
        {
            var plugin2 = _fixture.Create<TestKongConfig>();
            var plugin1 = _fixture.Create<TestKongConfig>();
            var body1 = _fixture.Create<PluginBody>();

            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService5",
                    Plugins = new List<IKongPluginConfig> { plugin2 }
                }
            };

            var newServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService5",
                    Plugins = new List<IKongPluginConfig> { plugin1 }
                }
            };

            var system = new ServicesProcessorSut();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            await system.Sut.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

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

            var system = new ServicesProcessorSut();

            var noServices = new List<KongService>();

            await system.Sut.Process(noServices, noServices, clusterConfig, fileConfig);

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

            var system = new ServicesProcessorSut();
            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin)).Returns(body);


            var noServices = new List<KongService>();

            await system.Sut.Process(noServices, noServices, clusterConfig, fileConfig);

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

            var system = new ServicesProcessorSut();
            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin)).Returns(body);


            var noServices = new List<KongService>();

            await system.Sut.Process(noServices, noServices, clusterConfig, fileConfig);

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

            var system = new ServicesProcessorSut();
            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin)).Returns(body);


            var noServices = new List<KongService>();

            await system.Sut.Process(noServices, noServices, clusterConfig, fileConfig);

            system.KongWriter.Verify(kong => kong.DeletePlugin(plugin.id), Times.Once());
        }
    }
}
