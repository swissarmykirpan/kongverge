using FluentAssertions;
using Kongverge.Common.Plugins.Custom;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class LoggingPluginTests
    {
        [Fact]
        public void TestRoundTripStartingAtConfigWithNoFields()
        {
            var configIn = new JELoggingConfig();
            var plugin = new JELoggingPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithAllFields()
        {
            var configIn = new JELoggingConfig
            {
                id = "test1",
                Host = "logging.com",
                KeepAlive = 1357,
                LogBody = true,
                MaxBodySize = 1234,
                Port = 9999,
                Protocol = "tcp",
                Timeout = 10000
            };

            var plugin = new JELoggingPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new JELoggingPlugin());
        }
    }
}
