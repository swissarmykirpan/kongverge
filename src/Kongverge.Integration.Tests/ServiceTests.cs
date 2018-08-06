using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
        public async Task AddServiceWorksAsExpected()
        {
            var service = new ServiceBuilder().AddDefaultTestService().Build();
            var kongAction = await _fixture.KongAdminService.AddService(service);
            _fixture.CleanUp.Add(service);
            kongAction.Succeeded.Should().BeTrue();
            kongAction.Result.Id.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task AddServiceWithRoutesWorksAsExpected()
        {
            var paths = new[] {"/health/check", "/foo/bar"};
            var serviceToAdd = new ServiceBuilder()
                .AddDefaultTestService()
                .WithPaths(paths)
                .Build();

            var kongAction = await _fixture.KongAdminService.AddService(serviceToAdd);
            _fixture.CleanUp.Add(serviceToAdd);

            kongAction.Succeeded.Should().BeTrue();
            var service = kongAction.Result;
            service.Id.Should().NotBeNullOrEmpty();
            service.Routes.First().Paths.Should().BeEquivalentTo(paths);
        }
    }
}
