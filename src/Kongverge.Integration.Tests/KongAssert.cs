using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.Services;

namespace Kongverge.Integration.Tests
{
    public static class KongAssert
    {
        public static async Task HasServiceWithId(this IKongAdminReader kongReader, string id)
        {
            var allServices = await kongReader.GetServices();
            var actual = allServices.FirstOrDefault(s => s.Id == id);

            actual.Should().NotBeNull($"Service with id '{id}' was not found");
        }

        public static async Task HasNoServiceWithId(this IKongAdminReader kongReader, string id)
        {
            var allServices = await kongReader.GetServices();
            var actual = allServices.FirstOrDefault(s => s.Id == id);

            actual.Should().BeNull($"Service with id '{id}' was found");
        }
    }
}
