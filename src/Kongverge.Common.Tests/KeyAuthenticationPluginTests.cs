using System.Collections.Generic;
using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class KeyAuthenticationPluginTests
    {
        [Fact]
        public void RoundTripFromConfigWithNoFields()
        {
            var configIn = new KeyAuthenticationConfig();
            var plugin = new KeyAuthenticationPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromConfigWithAllFields()
        {
            var configIn = new KeyAuthenticationConfig
            {
                id = "corr_id",
                Anonymous = "nobody",
                HideCredentials = true,
                KeyInBody = true,
                RunOnPreflight = true,
                KeyNames = new HashSet<string> { "foo", "bar", "fish" }
            };
            var plugin = new KeyAuthenticationPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new KeyAuthenticationPlugin());
        }
    }
}
