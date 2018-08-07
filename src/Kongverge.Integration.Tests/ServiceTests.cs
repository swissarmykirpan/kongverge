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
        public async Task UnknownServiceIsNotFound()
        {
            var noSuchId = Guid.NewGuid().ToString();

            await _fixture.KongAdminReader.HasNoServiceWithId(noSuchId);
        }

        [Fact]
        public async Task AddServiceWorksAsExpected()
        {
            var service = new ServiceBuilder().AddDefaultTestService().Build();
            var kongAction = await _fixture.KongAdminWriter.AddService(service);
            _fixture.CleanUp.Add(service);
            kongAction.Succeeded.Should().BeTrue();
            kongAction.Result.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.HasServiceWithId(kongAction.Result.Id);
        }

        [Fact]
        public async Task AddServiceWithRoutesWorksAsExpected()
        {
            var paths = new[] {"/health/check", "/foo/bar"};
            var serviceToAdd = new ServiceBuilder()
                .AddDefaultTestService()
                .WithPaths(paths)
                .Build();

            var kongAction = await _fixture.KongAdminWriter.AddService(serviceToAdd);
            _fixture.CleanUp.Add(serviceToAdd);

            kongAction.Succeeded.Should().BeTrue();
            var service = kongAction.Result;
            service.Id.Should().NotBeNullOrEmpty();
            service.Routes.First().Paths.Should().BeEquivalentTo(paths);

            await _fixture.KongAdminReader.HasServiceWithId(kongAction.Result.Id);
        }

        [Fact]
        public async Task DeleteServiceWorksAsExpected()
        {
            var service = await AddTestService();

            var deleteAction = await _fixture.KongAdminWriter.DeleteService(service);

            deleteAction.Succeeded.Should().BeTrue();
            await _fixture.KongAdminReader.HasNoServiceWithId(service.Id);
        }

        private async Task<KongService> AddTestService()
        {
            var service = new ServiceBuilder().AddDefaultTestService().Build();
            var addAction = await _fixture.KongAdminWriter.AddService(service);

            addAction.Succeeded.Should().BeTrue();
            addAction.Result.Id.Should().NotBeNullOrEmpty();

            await _fixture.KongAdminReader.HasServiceWithId(service.Id);

            return addAction.Result;
        }
    }
}
