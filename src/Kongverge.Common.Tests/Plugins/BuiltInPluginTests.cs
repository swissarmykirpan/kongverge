using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins.BuiltIn;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;
using Kongverge.KongPlugin;
using Kongverge.TestHelpers;

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
        protected override ISingleObjectBuilder<KeyAuthenticationConfig> Populate(ISingleObjectBuilder<KeyAuthenticationConfig> builder)
        {
            return builder.With(x => x.KeyNames, new HashSet<string>(Enumerable.Range(0, 3).Select(x => GetRandom.String(10))));
        }
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

        protected override ISingleObjectBuilder<RateLimitingConfig> Populate(ISingleObjectBuilder<RateLimitingConfig> builder)
        {
            return builder
                .With(x => x.Limit, Enumerable.Range(0, 3).Select(x => GetRandom.Int(10, 20)).ToArray())
                .With(x => x.WindowSize, Enumerable.Range(0, 3).Select(x => GetRandom.Int(10, 20)).ToArray());
        }
    }

    public class RequestTerminationPluginTests : PluginTests<RequestTerminationPlugin, RequestTerminationConfig>
    {
    }

    public class RequestTransformerAdvancedPluginTests : PluginTests<RequestTransformerAdvancedPlugin, RequestTransformerAdvancedConfig>
    {
        protected override ISingleObjectBuilder<RequestTransformerAdvancedConfig> Populate(ISingleObjectBuilder<RequestTransformerAdvancedConfig> builder)
        {
            return builder.PopulateRequestTransformerConfig();
        }
    }
}
