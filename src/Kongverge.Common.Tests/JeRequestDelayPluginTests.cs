using FluentAssertions;
using Kongverge.Common.Plugins.Custom;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class JeRequestDelayPluginTests
    {
        [Fact]
        public void TestRoundTripStartingAtConfigWithNoFields()
        {
            var configIn = new JeRequestDelayConfig();
            var plugin = new JeRequestDelayPlugin();
            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);
            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithAllFields()
        {
            var configIn = new JeRequestDelayConfig()
            {
                id = "test1",
                DelayMs = 100
            };
            var plugin = new JeRequestDelayPlugin();
            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);
            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new JeRequestDelayPlugin());
        }
    }
}
