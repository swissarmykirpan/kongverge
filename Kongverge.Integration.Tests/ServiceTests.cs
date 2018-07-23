using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using NSubstitute;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class ServiceTests : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;
        private readonly KongService _serviceToAdd =
            new KongService()
            {
                Name = $"testservice_{Guid.NewGuid().ToString()}",
                Host = "www.example.com",
                Port = 80
            };

        public ServiceTests(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
            _serviceToAdd.Name = $"testservice_{Guid.NewGuid().ToString()}";
        }

        [Fact]
        public async Task AddingServiceWorksAsExpected()
        {
            var kongAction = await _fixture.KongAdminService.AddService(_serviceToAdd);
            _fixture.CleanUp.Add(_serviceToAdd);
            kongAction.Succeeded.Should().BeTrue();
            kongAction.Result.Id.Should().NotBeNullOrEmpty();
        }
    }
}
