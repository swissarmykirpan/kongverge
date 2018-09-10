using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
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
    }
}
