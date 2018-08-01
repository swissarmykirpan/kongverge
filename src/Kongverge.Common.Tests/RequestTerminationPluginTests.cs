using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class RequestTerminationPluginTest
    {
        [Fact]
        public void RoundTripFromConfigWithNoFields()
        {
            var configIn = new RequestTerminationConfig();
            var plugin = new RequestTerminationPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromConfigWithAllFields()
        {
            var configIn = new RequestTerminationConfig
            {
                id = "rt_id",
                Message = "test stop",
                StatusCode = 404
            };
            var plugin = new RequestTerminationPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            // Have to provide at least the generator
            PluginHelpers.RoundTripFromBodyTest(new RequestTerminationPlugin());
        }

    }
}
