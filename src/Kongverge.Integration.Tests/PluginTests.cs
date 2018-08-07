using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class PluginTests : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;

        public PluginTests(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
        }

        [Fact]
        public async Task ServiceShouldNotHaveUndeclaredPlugins()
        {
            var service = new ServiceBuilder()
                .AddDefaultTestService()
                .Build();

            var kongAction = await _fixture.KongAdminWriter.AddService(service);
            _fixture.CleanUp.Add(service);
            kongAction.Succeeded.Should().BeTrue();

            var serviceId = kongAction.Result.Id;
            var serviceRead = await _fixture.KongAdminReader.GetService(serviceId);

            serviceRead.Extensions.Should().NotBeNull();
            serviceRead.Extensions.Should().BeEmpty();
        }
    }
}
