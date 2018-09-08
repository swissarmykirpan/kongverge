using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class ServiceTests : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;

        public ServiceTests(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
        }

        [Fact]
        public async Task DefaultServiceHasNoPlugins()
        {
            var service = _fixture.ServiceBuilder.CreateDefaultTestService().Build();
            await _fixture.AddServiceAndChildren(service);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(service.Id);

            serviceReadFromKong.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().BeEmpty();
        }

        [Fact]
        public void UnknownServiceIsNotFound()
        {
            var noSuchId = Guid.NewGuid().ToString();

            _fixture.KongAdminReader.ShouldNotHaveServiceWithId(noSuchId);
        }

        [Fact]
        public async Task AddServiceWorksAsExpected()
        {
            var service = _fixture.ServiceBuilder.CreateDefaultTestService().Build();
            await _fixture.AddServiceAndChildren(service);

            service.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.ShouldHaveServiceWithId(service.Id);
        }

        [Fact]
        public async Task AddServiceWithRoutesWillAddService()
        {
            var paths = new[] {"/health/check", "/foo/bar"};
            var service = _fixture.ServiceBuilder
                .CreateDefaultTestService()
                .WithRoutePaths(paths)
                .Build();

            await _fixture.AddServiceAndChildren(service);

            service.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.ShouldHaveServiceWithId(service.Id);
        }

        [Fact]
        public async Task AddServiceWithRoutesWillAddRoutesToKong()
        {
            var paths = new[] { "/health/check", "/foo/bar" };
            var service = _fixture.ServiceBuilder
                .CreateDefaultTestService()
                .WithRoutePaths(paths)
                .Build();

            await _fixture.AddServiceAndChildren(service);

            var routesReadFromKong = await _fixture.KongAdminReader.GetRoutes();
            var routesForService = routesReadFromKong.Where(x => x.Service.Id == service.Id).ToArray();
            routesForService.Should().HaveCount(1);
            routesForService.First().Paths.Should().BeEquivalentTo(paths);
        }
        
        [Fact]
        public async Task DeleteServiceWorksAsExpected()
        {
            var service = _fixture.ServiceBuilder.CreateDefaultTestService().Build();
            await _fixture.AddServiceAndChildren(service);
            
            service.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.ShouldHaveServiceWithId(service.Id);

            await _fixture.DeleteServiceWithChildren(service);
            _fixture.KongAdminReader.ShouldNotHaveServiceWithId(service.Id);
        }
    }
}
