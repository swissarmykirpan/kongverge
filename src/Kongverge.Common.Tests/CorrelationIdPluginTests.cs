using System.Collections.Generic;
using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn;
using Kongverge.KongPlugin;
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
                Template = "temp"
            };
            var plugin = new CorrelationIdPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            var plugin = new CorrelationIdPlugin();
            var bodyIn = new PluginBody(plugin.PluginName, new Dictionary<string, object>());

            var bodyOut = PluginHelpers.RoundTripFromBody(plugin, bodyIn);

            bodyOut.Should().NotBeNull();
            bodyOut.name.Should().Be(bodyIn.name);
        }
    }
}
