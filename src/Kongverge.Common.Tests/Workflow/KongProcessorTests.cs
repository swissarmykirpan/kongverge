using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
using Kongverge.Common.Tests.Helpers;
using Kongverge.KongPlugin;
using Moq;
using Xunit;

namespace Kongverge.Common.Tests.Workflow
{
    public class KongProcessorTests
    {
        [Fact]
        public async Task NoOpWithNoServicesWorks()
        {
            var system = new KongProcessorEnvironment();

            var existingServices = new List<KongService>();
            var newServices = new List<KongService>();

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.VerifyNoActionTaken();
        }

        [Fact]
        public async Task NoOpWithOneServicesWorks()
        {
            var system = new KongProcessorEnvironment();

            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Name = "service1"
                }
            };

            var newServices = new List<KongService>
            {
                new KongService
                {
                    Name = "service1"
                }
            };

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.VerifyNoActionTaken();
        }

        [Fact]
        public async Task CanAddService()
        {
            var system = new KongProcessorEnvironment();

            var existingServices = new List<KongService>();
            var newServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService_A"
                }
            };
            
            system.KongWriter.Setup(e => e.AddService(It.IsAny<KongService>()))
                .ReturnsAsync(newServices.First());

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k =>
                k.AddService(It.IsAny<KongService>()), Times.Once);
            system.KongWriter.Verify(k =>
                k.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Never);
        }

        [Fact]
        public async Task CanRemoveService()
        {
            var system = new KongProcessorEnvironment();

            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService_A"
                }
            };

            var newServices = new List<KongService>();

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k =>
                k.AddService(It.IsAny<KongService>()), Times.Never);
            system.KongWriter.Verify(k =>
                k.DeleteService(It.IsAny<string>()), Times.Once);
            system.KongWriter.Verify(k =>
                k.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Never);
        }

        [Fact]
        public async Task ConvergeRoutes_WillAddMissingRoutes()
        {
            var system = new KongProcessorEnvironment();
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

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k => k.AddRoute(newServices[0], route1), Times.Once());
        }

        [Fact]
        public async Task ConvergeRoutes_WillRemoveExcessRoutes()
        {
            var system = new KongProcessorEnvironment();
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

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k => k.DeleteRoute(route1.Id), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillRemoveExcessPlugins()
        {
            var fixture = new Fixture();
            var plugin2 = fixture.Create<TestKongConfig>();
            var plugin1 = fixture.Create<OtherTestKongConfig>();

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

            var system = new KongProcessorEnvironment();

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k => k.DeletePlugin(plugin1.id), Times.Once());
            system.KongWriter.Verify(k => k.DeletePlugin(plugin2.id), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillAddMissingPlugins()
        {
            var fixture = new Fixture();
            var plugin2 = fixture.Create<TestKongConfig>();
            var plugin1 = fixture.Create<OtherTestKongConfig>();
            var body1 = fixture.Create<PluginBody>();
            var body2 = fixture.Create<PluginBody>();

            var system = new KongProcessorEnvironment();

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

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());
            system.KongWriter.Verify(k => k.UpsertPlugin(body2), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillUpdateChangedPlugins()
        {
            var fixture = new Fixture();
            var plugin2 = fixture.Create<TestKongConfig>();
            var plugin1 = fixture.Create<TestKongConfig>();
            var body1 = fixture.Create<PluginBody>();

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

            var system = new KongProcessorEnvironment();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());
        }
    }
}
