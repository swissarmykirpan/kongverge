using System;
using System.Collections.Generic;
using System.Text;
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
            var fixture = new Fixture();
            var plugin1 = fixture.Create<OtherTestKongConfig>();
            var body1 = fixture.Create<PluginBody>();

            var system = new KongProcessorEnvironment();

            system.KongPluginCollection.Setup(e => e.CreatePluginBody(plugin1)).Returns(body1);

            var existingServices = new List<KongService>();

            var newServices = new List<KongService>
            {
                new KongService
                {
                    Name = "TestService1",
                    Routes = new List<KongRoute>
                    {
                        new KongRoute
                        {
                            Methods = new List<string> { "GET "},
                            Paths =  new List<string> { "/foo/bar "},
                            Plugins = new List<IKongPluginConfig>
                                {
                                    plugin1
                                }
                        }
                    }
                }
            };

            await system.Processor.Process(existingServices, newServices, new GlobalConfig(), new GlobalConfig());

            system.KongWriter.Verify(k =>
                k.AddService(It.IsAny<KongService>()), Times.Once);
            system.KongWriter.Verify(k =>
                k.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Once);
            system.KongWriter.Verify(k => k.UpsertPlugin(body1), Times.Once());
        }
    }
}
