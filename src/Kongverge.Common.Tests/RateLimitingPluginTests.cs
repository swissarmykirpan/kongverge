
using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class RateLimitingPluginPluginTests
    {
        [Fact]
        public void RoundTripFromConfigWithNoFields()
        {
            var configIn = new RateLimitingConfig
            {
                Limit = new int[0],
                WindowSize = new int[0]
            };

            var plugin = new RateLimitingPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromConfigWithAllFields()
        {
            var configIn = new RateLimitingConfig
            {
                id = "corr_id",
                Identifier = "rat rat rat",
                Limit = new[] { 42 },
                WindowSize = new[] { 1234 }
            };
            var plugin = new RateLimitingPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new RateLimitingPlugin());
        }
    }
}
