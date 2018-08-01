using FluentAssertions;
using Kongverge.Common.Plugins.Custom;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class JeTcpLogPluginTests
    {
        [Fact]
        public void TestRoundTripStartingAtConfigWithNoFields()
        {
            var configIn = new JeTcpLogConfig();
            var plugin = new JeTcpLogPlugin();
            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);
            configOut.IsExactMatch(configIn).Should().BeTrue();
        }
        [Fact]
        public void TestRoundTripStartingAtConfigWithAllFields()
        {
            var configIn = new JeTcpLogConfig
            {
                id = "test1",
                Host = "logging.com",
                KeepAlive = 1357,
                LogBody = true,
                MaxBodySize = 1234,
                Port = 9999,
                Timeout = 10000
            };
            var plugin = new JeTcpLogPlugin();
            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);
            configOut.IsExactMatch(configIn).Should().BeTrue();
        }
        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new JeTcpLogPlugin());
        }
    }
}