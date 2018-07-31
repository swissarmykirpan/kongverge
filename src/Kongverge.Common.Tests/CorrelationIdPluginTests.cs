using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class CorrelationIdPluginTests
    {
        [Fact]
        public void TestRoundTripStartingAtConfigWithNoFields()
        {
            var configIn = new CorrelationIdConfig();
            var plugin = new CorrelationIdPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithAllFields()
        {
            var configIn = new CorrelationIdConfig
            {
                EchoDownstream = true,
                Header = "hdr",
                id = "someId",
                Template = CorrelationIdGenerator.Counter
            };
            var plugin = new CorrelationIdPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }
    }
}
