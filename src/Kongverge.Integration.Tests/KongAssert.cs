using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Kongverge.Common.Services;
using Kongverge.KongPlugin;

namespace Kongverge.Integration.Tests
{
    public static class KongAssert
    {
        public static async Task ShouldHaveServiceWithId(this IKongAdminReader kongReader, string id)
        {
            var actual = await kongReader.GetService(id);
            actual.Should().NotBeNull($"Service with id '{id}' was not found");
        }

        public static async Task ShouldNotHaveServiceWithId(this IKongAdminReader kongReader, string id)
        {
            var actual = await kongReader.GetService(id);
            actual.Should().BeNull($"Service with id '{id}' was found");
        }

        public static T ShouldHaveOnePlugin<T>(this ExtendibleKongObject service) where T : IKongPluginConfig
        {
            service.Should().NotBeNull();
            service.Plugins.Should().NotBeNull();
            service.Plugins.Should().HaveCount(1);
            service.Plugins[0].Should().BeOfType<T>();

            return (T)service.Plugins[0];
        }
    }
}
