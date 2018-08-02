using FluentAssertions;
using Kongverge.Common.Plugins.Custom;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class JeJustSayingDefaultsPluginTests
    {
        [Fact]
        public void TestRoundTripStartingAtConfigWithNoFields()
        {
            var configIn = new JeJustSayingDefaultsConfig();
            var plugin = new JeJustSayingDefaultsPlugin();
            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);
            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithAllFields()
        {
            var configIn = new JeJustSayingDefaultsConfig
            {
                id = "test1",
                RaisingComponent = "myComponent",
                Tenant = "all"
            };
            var plugin = new JeJustSayingDefaultsPlugin();
            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);
            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new JeJustSayingDefaultsPlugin());
        }
    }
}
