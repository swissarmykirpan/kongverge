using System.Collections.Generic;
using FizzWare.NBuilder;
using Kongverge.Common.Plugins.BuiltIn;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;
using Kongverge.Common.Tests.Plugins;

namespace Kongverge.Integration.Tests
{
    public class CorrelationIdPluginTests : PluginTests<CorrelationIdConfig>
    {
        public CorrelationIdPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }

    public class KeyAuthenticationPluginTests : PluginTests<KeyAuthenticationConfig>
    {
        public KeyAuthenticationPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }

    public class RateLimitingPluginTests : PluginTests<RateLimitingConfig>
    {
        public RateLimitingPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }

    public class RequestTerminationPluginTests : PluginTests<RequestTerminationConfig>
    {
        public RequestTerminationPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }

    public class RequestTransformerAdvancedPluginTests : RequestTransformerPluginTests<RequestTransformerAdvancedConfig>
    {
        public RequestTransformerAdvancedPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }

    public abstract class RequestTransformerPluginTests<TPluginConfig> : PluginTests<TPluginConfig>
        where TPluginConfig : RequestTransformerConfig, new()
    {
        protected RequestTransformerPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }

        protected override IEnumerable<TPluginConfig> Permutations
        {
            get
            {
                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Replace, Builder<AdvancedTransformReplace>.CreateNew().Build())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Add, BuilderExtensions.RandomBaseConfig<AdvancedTransform>())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Append, BuilderExtensions.RandomBaseConfig<AdvancedTransform>())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Remove, BuilderExtensions.RandomRemoveConfig())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Replace, BuilderExtensions.RandomBaseConfig<AdvancedTransformReplace>())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Rename, BuilderExtensions.RandomBaseConfig<AdvancedTransform>())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .PopulateRequestTransformerConfig()
                    .Build();
            }
        }
    }
}
