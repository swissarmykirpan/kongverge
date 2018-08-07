using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.Services;

namespace Kongverge.Integration.Tests
{
    public static class KongAssert
    {
        public static async Task HasServiceWithId(this IKongAdminReader kongReader, string id)
        {
            var actual = await kongReader.GetService(id);
            actual.Should().NotBeNull($"Service with id '{id}' was not found");
        }

        public static async Task HasNoServiceWithId(this IKongAdminReader kongReader, string id)
        {
            var actual = await kongReader.GetService(id);
            actual.Should().BeNull($"Service with id '{id}' was found");
        }
    }
}
