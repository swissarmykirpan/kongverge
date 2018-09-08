using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
using Moq;
using Xunit;

namespace Kongverge.Common.Tests.Workflow
{
    public class KongProcessorTests : KongProcessorTestsBase
    {
        [Fact]
        public async Task NoOpWithNoServicesWorks()
        {
            await Processor.Process(Array.Empty<KongService>(), Array.Empty<KongService>(), new ExtendibleKongObject(), new ExtendibleKongObject());

            VerifyNoActionTaken();
        }

        [Fact]
        public async Task NoOpWithOneServicesWorks()
        {
            var name = this.Create<string>();

            var existingServices = new[]
            {
                new KongService
                {
                    Name = name
                }
            };

            var targetServices = new[]
            {
                new KongService
                {
                    Name = name
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            VerifyNoActionTaken();
        }

        [Fact]
        public async Task CanAddService()
        {
            var existingServices = Array.Empty<KongService>();
            var targetServices = new[]
            {
                new KongService
                {
                    Name = this.Create<string>()
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.AddService(targetServices.Single()), Times.Once);
            KongWriter.Verify(x => x.UpdateService(It.IsAny<KongService>()), Times.Never);
            KongWriter.Verify(x => x.AddRoute(It.IsAny<string>(), It.IsAny<KongRoute>()), Times.Never);
            KongWriter.Verify(x => x.UpsertPlugin(It.IsAny<KongPlugin>()), Times.Never);

            VerifyNoDeletes();
        }

        [Fact]
        public async Task CanRemoveService()
        {
            var existingServices = new[]
            {
                new KongService
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "TestService_A"
                }
            };

            var targetServices = Array.Empty<KongService>();

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.UpdateService(It.IsAny<KongService>()), Times.Never);
            KongWriter.Verify(x => x.DeleteService(existingServices.Single().Id), Times.Once);
            KongWriter.Verify(x => x.AddRoute(It.IsAny<string>(), It.IsAny<KongRoute>()), Times.Never);
            KongWriter.Verify(x => x.UpsertPlugin(It.IsAny<KongPlugin>()), Times.Never);

            VerifyNoAdds();
        }

        [Fact]
        public async Task ConvergeRoutes_WillAddMissingRoutes()
        {
            var name = this.Create<string>();

            var existingServices = new[]
            {
                new KongService
                {
                    Name = name
                }
            };

            var targetServices = new[]
            {
                new KongService
                {
                    Name = name,
                    Routes = new[]
                    {
                        Build<KongRoute>().Without(x => x.Id).Create()
                    }
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.AddRoute(targetServices[0].Id, targetServices[0].Routes[0]), Times.Once());
        }

        [Fact]
        public async Task ConvergeRoutes_WillRemoveExcessRoutes()
        {
            var name = this.Create<string>();

            var existingServices = new[]
            {
                new KongService
                {
                    Name = name,
                    Routes = new[]
                    {
                        this.Create<KongRoute>()
                    }
                }
            };

            var targetServices = new[]
            {
                new KongService
                {
                    Name = name,
                    Routes = Array.Empty<KongRoute>()
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.DeleteRoute(existingServices[0].Routes[0].Id), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillRemoveExcessPlugins()
        {
            var name = this.Create<string>();

            var existingServices = new[]
            {
                new KongService
                {
                    Name = name,
                    Plugins = new[]
                    {
                        this.Create<KongPlugin>(),
                        this.Create<KongPlugin>()
                    }
                }
            };

            var targetServices = new[]
            {
                new KongService
                {
                    Name = name
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.DeletePlugin(existingServices[0].Plugins[0].Id), Times.Once());
            KongWriter.Verify(x => x.DeletePlugin(existingServices[0].Plugins[1].Id), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillAddMissingPlugins()
        {
            var name = this.Create<string>();

            var existingServices = new[]
            {
                new KongService
                {
                    Name = name
                }
            };

            var targetServices = new[]
            {
                new KongService
                {
                    Name = name,
                    Plugins = new[]
                    {
                        Build<KongPlugin>().Without(x => x.Id).Create(),
                        Build<KongPlugin>().Without(x => x.Id).Create()
                    }
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.UpsertPlugin(targetServices[0].Plugins[0]), Times.Once());
            KongWriter.Verify(x => x.UpsertPlugin(targetServices[0].Plugins[0]), Times.Once());
        }

        [Fact]
        public async Task ConvergePlugins_WillUpdateChangedPlugins()
        {
            var name = this.Create<string>();
            var plugin = this.Create<KongPlugin>();
            var changedPlugin = Build<KongPlugin>().With(x => x.Name, plugin.Name).Create();

            var existingServices = new[]
            {
                new KongService
                {
                    Name = name,
                    Plugins = new[] { plugin }
                }
            };

            var targetServices = new[]
            {
                new KongService
                {
                    Name = name,
                    Plugins = new[] { changedPlugin }
                }
            };

            await Processor.Process(existingServices, targetServices, new ExtendibleKongObject(), new ExtendibleKongObject());

            KongWriter.Verify(x => x.UpsertPlugin(changedPlugin), Times.Once());
        }
    }
}
