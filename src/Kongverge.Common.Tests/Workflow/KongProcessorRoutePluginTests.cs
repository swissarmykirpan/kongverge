using System;
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
    public class KongProcessorRoutePluginTests
    {
        [Fact]
        public async Task WhenServiceRouteAndPluginAreAllNew_TheyAreAllAdded()
        {
            var routeId = Guid.NewGuid().ToString();
            var fixture = new Fixture();
            var plugin1 = fixture.Create<OtherTestKongConfig>();
            var body1 = fixture.Create<PluginBody>();

            var system = new KongProcessorEnvironment();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            var existingServices = new List<KongService>();

            var newServices = ServiceWithRouteAndPlugin(routeId, plugin1);

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k =>
                k.AddService(It.IsAny<KongService>()), Times.Once);
            system.KongWriter.Verify(k =>
                k.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Once);
            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());

            system.VerifyNoDeletes();
        }

        [Fact]
        public async Task WhenRouteAndPluginAreNew_TheyAreAdded()
        {
            var routeId = Guid.NewGuid().ToString();
            var fixture = new Fixture();
            var plugin1 = fixture.Create<OtherTestKongConfig>();
            var body1 = fixture.Create<PluginBody>();

            var system = new KongProcessorEnvironment();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService1"
                }
            };

            var newServices = ServiceWithRouteAndPlugin(routeId, plugin1);

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k =>
                k.AddService(It.IsAny<KongService>()), Times.Never);
            system.KongWriter.Verify(k =>
                k.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Once);
            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());

            system.VerifyNoDeletes();
        }

        [Fact]
        public async Task WhenPluginIsNew_ItIsAdded()
        {
            var fixture = new Fixture();
            var plugin1 = fixture.Create<OtherTestKongConfig>();
            var body1 = fixture.Create<PluginBody>();
            var routeId = Guid.NewGuid().ToString();

            var system = new KongProcessorEnvironment();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService1",
                    Routes = new List<KongRoute>
                    {
                        new KongRoute
                        {
                            Id = routeId,
                            Methods = new List<string> { "GET "},
                            Paths =  new List<string> { "/foo/bar "}
                            // no plugin in the existing state
                        }
                    }
                }
            };

            var newServices = ServiceWithRouteAndPlugin(routeId, plugin1);

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k =>
                k.AddService(It.IsAny<KongService>()), Times.Never);
            system.KongWriter.Verify(k =>
                k.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Never);
            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());

            system.VerifyNoDeletes();
        }

        [Fact]
        public async Task WhenRoutePluginIsRemoved_ItIsDeleted()
        {
            var fixture = new Fixture();
            var plugin1 = fixture.Create<OtherTestKongConfig>();
            var body1 = fixture.Create<PluginBody>();
            var routeId = Guid.NewGuid().ToString();

            var system = new KongProcessorEnvironment();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            var existingServices = ServiceWithRouteAndPlugin(routeId, plugin1);

            var newServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService1",
                    Routes = new List<KongRoute>
                    {
                        new KongRoute
                        {
                            Id = routeId,
                            Methods = new List<string> { "GET "},
                            Paths =  new List<string> { "/foo/bar "}
                            // no plugin in the new state
                        }
                    }
                }
            };

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.VerifyNoAdds();

            system.KongWriter.Verify(k => k.DeleteService(It.IsAny<string>()), Times.Never);
            system.KongWriter.Verify(k => k.DeleteRoute(It.IsAny<string>()), Times.Never);
            system.KongWriter.Verify(k => k.DeletePlugin(plugin1.id), Times.Once());
        }

        [Fact]
        public async Task WhenRoutePluginAndRouteAreRemoved_TheyAreDeleted()
        {
            var fixture = new Fixture();
            var plugin1 = fixture.Create<OtherTestKongConfig>();
            var body1 = fixture.Create<PluginBody>();
            var routeId = Guid.NewGuid().ToString();

            var system = new KongProcessorEnvironment();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            var existingServices = ServiceWithRouteAndPlugin(routeId, plugin1);

            var newServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService1"
                    // no routes in new state
                }
            };

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.VerifyNoAdds();

            system.KongWriter.Verify(k => k.UpsertPlugin(It.IsAny<PluginBody>()), Times.Never);

            system.KongWriter.Verify(k => k.DeleteService(It.IsAny<string>()), Times.Never);
            system.KongWriter.Verify(k => k.DeleteRoute(It.IsAny<string>()), Times.Once);
        }

        private static List<KongService> ServiceWithRouteAndPlugin(string routeId, OtherTestKongConfig plugin1)
        {
            return new List<KongService>
            {
                new KongService
                {
                    Name = "TestService1",
                    Routes = new List<KongRoute>
                    {
                        new KongRoute
                        {
                            Id = routeId,
                            Methods = new List<string> { "GET " },
                            Paths = new List<string> { "/foo/bar " },
                            Plugins = new List<IKongPluginConfig>
                            {
                                plugin1
                            }
                        }
                    }
                }
            };
        }
    }
}
