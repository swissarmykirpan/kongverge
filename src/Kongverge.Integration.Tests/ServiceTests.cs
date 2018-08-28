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
            var kongServiceAdded = await _fixture.AddServiceAndChildren(service);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

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
            var addedService = await _fixture.AddServiceAndChildren(service);

            addedService.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.ShouldHaveServiceWithId(addedService.Id);
        }

        [Fact]
        public async Task AddServiceWithRoutesWillAddService()
        {
            var paths = new[] {"/health/check", "/foo/bar"};
            var serviceToAdd = _fixture.ServiceBuilder
                .CreateDefaultTestService()
                .WithRoutePaths(paths)
                .Build();

            var addedService = await _fixture.AddServiceAndChildren(serviceToAdd);

            addedService.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.ShouldHaveServiceWithId(addedService.Id);
        }

        [Fact]
        public async Task AddServiceWithRoutesWillAddRoutesToKong()
        {
            var paths = new[] { "/health/check", "/foo/bar" };
            var serviceToAdd = _fixture.ServiceBuilder
                .CreateDefaultTestService()
                .WithRoutePaths(paths)
                .Build();

            var addedService = await _fixture.AddServiceAndChildren(serviceToAdd);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(addedService.Id);
            serviceReadFromKong.Routes.Should().HaveCount(1);
            serviceReadFromKong.Routes.First().Paths.Should().BeEquivalentTo(paths);
        }
        
        [Fact]
        public async Task DeleteServiceWorksAsExpected()
        {
            var service = _fixture.ServiceBuilder.CreateDefaultTestService().Build();
            var addedService = await _fixture.AddServiceAndChildren(service);
            
            addedService.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.ShouldHaveServiceWithId(addedService.Id);

            await _fixture.DeleteServiceWithChildren(addedService);
            _fixture.KongAdminReader.ShouldNotHaveServiceWithId(addedService.Id);
        }
    }
}
