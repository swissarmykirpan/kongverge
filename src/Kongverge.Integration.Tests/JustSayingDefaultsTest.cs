using System.Threading.Tasks;
using Kongverge.Common.Plugins.Custom;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class JustSayingDefaultsTest : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;

        public JustSayingDefaultsTest(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
        }

        [Fact]
        public async Task ServiceCanHaveJustSayingDefaultsPluginWithFullConfig()
        {
            var plugin = new JeJustSayingDefaultsConfig()
            {   Tenant = "Bw",
                RaisingComponent = "TestComponent"
            };

            await _fixture.ShouldRoundTripPlugInToKong(plugin);

        }
    }
}
