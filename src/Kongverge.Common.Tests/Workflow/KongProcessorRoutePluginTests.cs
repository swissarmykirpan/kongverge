using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
using Moq;
using Xunit;

namespace Kongverge.Common.Tests.Workflow
{
    public class KongProcessorRoutePluginTests : KongProcessorTestsBase
    {
        [Fact]
        public async Task WhenServiceRouteAndPluginAreAllNew_TheyAreAllAdded()
        {
            var plugin = this.Create<KongPlugin>();
            
            var targetServices = ServiceWithRouteAndPlugin(plugin);

            await Processor.Process(new KongService[0], targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.AddService(targetServices[0]), Times.Once);
            KongWriter.Verify(x => x.AddRoute(targetServices[0].Id, targetServices[0].Routes[0]), Times.Once);
            KongWriter.Verify(x => x.UpsertPlugin(plugin), Times.Once());

            VerifyNoDeletes();
        }

        [Fact]
        public async Task WhenRouteAndPluginAreNew_TheyAreAdded()
        {
            var plugin = this.Create<KongPlugin>();
            
            var targetServices = ServiceWithRouteAndPlugin(plugin);
            var existingServices = new[]
            {
                new KongService
                {
                    Id = this.Create<string>(),
                    Name = targetServices[0].Name
                }
            };
            
            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.AddService(targetServices[0]), Times.Never);
            KongWriter.Verify(x => x.AddRoute(targetServices[0].Id, targetServices[0].Routes[0]), Times.Once);
            KongWriter.Verify(x => x.UpsertPlugin(plugin), Times.Once());

            VerifyNoDeletes();
        }

        [Fact]
        public async Task WhenPluginIsNew_ItIsAdded()
        {
            var plugin = this.Create<KongPlugin>();

            var targetServices = ServiceWithRouteAndPlugin( plugin);
            var existingServices = new List<KongService>
            {
                new KongService
                {
                    Id = this.Create<string>(),
                    Name = targetServices[0].Name,
                    Routes = new List<KongRoute>
                    {
                        new KongRoute
                        {
                            Id = this.Create<string>(),
                            Methods = new[] { targetServices[0].Routes[0].Methods.Single() },
                            Paths =  new[] { targetServices[0].Routes[0].Paths.Single() }
                            // no plugin in the existing state
                        }
                    }
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.AddService(It.IsAny<KongService>()), Times.Never);
            KongWriter.Verify(x => x.AddRoute(It.IsAny<string>(), It.IsAny<KongRoute>()), Times.Never);
            KongWriter.Verify(x => x.UpsertPlugin(plugin), Times.Once());

            VerifyNoDeletes();
        }

        [Fact]
        public async Task WhenRoutePluginIsRemoved_ItIsDeleted()
        {
            var plugin = this.Create<KongPlugin>();

            var existingServices = ServiceWithRouteAndPlugin(plugin, true);
            var targetServices = new List<KongService>
            {
                new KongService
                {
                    Name = existingServices[0].Name,
                    Routes = new List<KongRoute>
                    {
                        new KongRoute
                        {
                            Methods = new[] { existingServices[0].Routes[0].Methods.Single() },
                            Paths =  new[] { existingServices[0].Routes[0].Paths.Single() }
                            // no plugin in the new state
                        }
                    }
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            VerifyNoAdds();

            KongWriter.Verify(x => x.DeleteService(It.IsAny<string>()), Times.Never);
            KongWriter.Verify(x => x.DeleteRoute(It.IsAny<string>()), Times.Never);
            KongWriter.Verify(x => x.DeletePlugin(plugin.Id), Times.Once());
        }

        [Fact]
        public async Task WhenRoutePluginAndRouteAreRemoved_TheyAreDeleted()
        {
            var plugin = this.Create<KongPlugin>();

            var existingServices = ServiceWithRouteAndPlugin(plugin, true);
            var targetServices = new List<KongService>
            {
                new KongService
                {
                    Name = existingServices[0].Name,
                    // no routes in new state
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            VerifyNoAdds();

            KongWriter.Verify(x => x.UpsertPlugin(It.IsAny<KongPlugin>()), Times.Never);
            KongWriter.Verify(x => x.DeleteService(It.IsAny<string>()), Times.Never);
            KongWriter.Verify(x => x.DeleteRoute(existingServices[0].Routes[0].Id), Times.Once);
        }

        private IReadOnlyList<KongService> ServiceWithRouteAndPlugin(KongPlugin plugin, bool existing = false)
        {
            if (!existing)
            {
                plugin.Id = null;
            }

            return new[]
            {
                new KongService
                {
                    Name = this.Create<string>(),
                    Routes = new[]
                    {
                        new KongRoute
                        {
                            Id = existing ? this.Create<string>() : null,
                            Methods = new[] { this.Create<string>() },
                            Paths = new[] { this.Create<string>() },
                            Plugins = new[]
                            {
                                plugin
                            }
                        }
                    }
                }
            };
        }
    }
}
