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
                HeaderName = "hdr",
                id = "someId",
                Generator = CorrelationIdGenerator.Counter
            };
            var plugin = new CorrelationIdPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }


        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            // Have to provide at least the generator
            var bodyOut = PluginHelpers.RoundTripFromBodyTest(new CorrelationIdPlugin(), new Dictionary<string, object>{
                { "generator", "uuid" }
            });

            bodyOut.config.ReadString("generator").Should().Be("uuid");
        }
    }
}
