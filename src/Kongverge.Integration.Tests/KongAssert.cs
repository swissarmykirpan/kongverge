using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.Common.Services;

namespace Kongverge.Integration.Tests
{
    public static class KongAssert
    {
        public static async Task ShouldHaveServiceWithId(this IKongAdminReader kongReader, string id)
        {
            var actual = await kongReader.GetService(id);
            actual.Should().NotBeNull($"Service with id '{id}' was not found");
        }

        public static void ShouldNotHaveServiceWithId(this IKongAdminReader kongReader, string id)
        {
            Func<Task> action = async () => await kongReader.GetService(id);
            action.Should().Throw<KongException>().Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public static KongPlugin ShouldHaveOnePlugin(this ExtendibleKongObject service, string name)
        {
            service.Should().NotBeNull();
            service.Plugins.Should().NotBeNull();
            service.Plugins.Should().HaveCount(1);
            service.Plugins.Single().Name.Should().Be(name);

            return service.Plugins.Single();
        }
    }
}
