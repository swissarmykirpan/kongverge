using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class CorrelationIdPluginTests
    {
        [Fact]
        public void RoundTripFromConfigWithNoFields()
        {
            var configIn = new CorrelationIdConfig();
            var plugin = new CorrelationIdPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromConfigWithAllFields()
        {
            var configIn = new CorrelationIdConfig
            {
                EchoDownstream = true,
                Header = "hdr",
                id = "someId",
                Template = Template.Counter
            };
            var plugin = new CorrelationIdPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new CorrelationIdPlugin());
        }
    }
}
