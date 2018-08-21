using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.Common.Services;
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
            var service = new ServiceBuilder().AddDefaultTestService().Build();
            var kongServiceAdded = await _fixture.AddServiceAndChildren(service);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            serviceReadFromKong.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().BeEmpty();
        }

        [Fact]
        public async Task UnknownServiceIsNotFound()
        {
            var noSuchId = Guid.NewGuid().ToString();

            await _fixture.KongAdminReader.ShouldNotHaveServiceWithId(noSuchId);
        }

        [Fact]
        public async Task AddServiceWorksAsExpected()
        {
            var service = new ServiceBuilder().AddDefaultTestService().Build();
            var kongAction = await _fixture.KongAdminWriter.AddService(service);
            _fixture.CleanUp.Add(service);
            kongAction.ShouldSucceed();
            kongAction.Result.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.ShouldHaveServiceWithId(kongAction.Result.Id);
        }

        [Fact]
        public async Task AddServiceWithRoutesWillAddService()
        {
            var paths = new[] {"/health/check", "/foo/bar"};
            var serviceToAdd = new ServiceBuilder()
                .AddDefaultTestService()
                .WithRoutePaths(paths)
                .Build();

            var kongAction = await AddServiceAndRoutes(serviceToAdd);
            _fixture.CleanUp.Add(serviceToAdd);

            kongAction.ShouldSucceed();
            var service = kongAction.Result;
            service.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.ShouldHaveServiceWithId(kongAction.Result.Id);
        }

        [Fact]
        public async Task AddServiceWithRoutesWillAddRoutesToKong()
        {
            var paths = new[] { "/health/check", "/foo/bar" };
            var serviceToAdd = new ServiceBuilder()
                .AddDefaultTestService()
                .WithRoutePaths(paths)
                .Build();

            var kongAction = await AddServiceAndRoutes(serviceToAdd);
            _fixture.CleanUp.Add(serviceToAdd);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongAction.Result.Id);
            serviceReadFromKong.Routes.Should().HaveCount(1);
            serviceReadFromKong.Routes.First().Paths.Should().BeEquivalentTo(paths);
        }
        
        [Fact]
        public async Task DeleteServiceWorksAsExpected()
        {
            var service = await AddTestService();

            var deleteAction = await _fixture.KongAdminWriter.DeleteService(service.Id);

            deleteAction.ShouldSucceed();
            await _fixture.KongAdminReader.ShouldNotHaveServiceWithId(service.Id);
        }

        private async Task<KongService> AddTestService()
        {
            var service = new ServiceBuilder().AddDefaultTestService().Build();
            var addAction = await _fixture.KongAdminWriter.AddService(service);

            addAction.ShouldSucceed();
            addAction.Result.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.ShouldHaveServiceWithId(service.Id);

            return addAction.Result;
        }

        private async Task<KongAction<KongService>> AddServiceAndRoutes(KongService service)
        {
            var kongAction = await _fixture.KongAdminWriter.AddService(service);
            kongAction.ShouldSucceed();

            foreach (var route in service.Routes)
            {
                var routeResult = await _fixture.KongAdminWriter.AddRoute(service, route);
                routeResult.ShouldSucceed();
            }

            return kongAction;
        }
    }
}
