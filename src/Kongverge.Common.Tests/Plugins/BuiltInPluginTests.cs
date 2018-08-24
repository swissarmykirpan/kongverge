using System.Collections.Generic;
using FluentAssertions;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins.BuiltIn;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Tests.Plugins
{
    public class CorrelationIdPluginTests : PluginTests<CorrelationIdPlugin, CorrelationIdConfig>
    {
        protected override Dictionary<string, object> EmptyConfig => new Dictionary<string, object>
        {
            { "generator", default(CorrelationIdGenerator).ToJsonString() }
        };

        protected override void AssertEmptyConfig(Dictionary<string, object> config)
        {
            config.ReadString("generator").Should().Be(default(CorrelationIdGenerator).ToJsonString());
        }
    }

    public class KeyAuthenticationPluginTests : PluginTests<KeyAuthenticationPlugin, KeyAuthenticationConfig>
    {
    }

    public class RateLimitingPluginTests : PluginTests<RateLimitingPlugin, RateLimitingConfig>
    {
        protected override Dictionary<string, object> EmptyConfig => new Dictionary<string, object>
        {
            { "identifier", default(RateLimitingIdentifier).ToJsonString() }
        };

        protected override void AssertEmptyConfig(Dictionary<string, object> config)
        {
            config.ReadString("identifier").Should().Be(default(RateLimitingIdentifier).ToJsonString());
        }
    }

    public class RequestTerminationPluginTests : PluginTests<RequestTerminationPlugin, RequestTerminationConfig>
    {
    }

    public class RequestTransformerAdvancedPluginTests : PluginTests<RequestTransformerAdvancedPlugin, RequestTransformerAdvancedConfig>
    {
    }
}
