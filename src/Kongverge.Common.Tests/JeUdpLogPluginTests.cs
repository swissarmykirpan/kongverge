using FluentAssertions;
using Kongverge.Common.Plugins.Custom;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class JeUdpLogPluginTests
    {
        [Fact]
        public void TestRoundTripStartingAtConfigWithNoFields()
        {
            var configIn = new JeUdpLogConfig();
            var plugin = new JeUdpLogPlugin();
            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);
            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithAllFields()
        {
            var configIn = new JeUdpLogConfig
            {
                id = "test1",
                Host = "logging.com",
                LogBody = true,
                MaxBodySize = 1234,
                Port = 9999,
                Timeout = 10000
            };
            var plugin = new JeUdpLogPlugin();
            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);
            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new JeUdpLogPlugin());
        }
    }
}
