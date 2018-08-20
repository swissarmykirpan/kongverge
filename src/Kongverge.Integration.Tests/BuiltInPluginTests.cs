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
        where TPluginConfig : BaseRequestTransformerConfig, new()
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
                    .With(x => x.Replace, Builder<RequestTransformerAdvancedTransformReplace>.CreateNew().Build())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Add, BuilderExtensions.RandomBaseConfig<RequestTransformerAdvancedTransformBase>())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Append, BuilderExtensions.RandomBaseConfig<RequestTransformerAdvancedTransformBase>())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Remove, BuilderExtensions.RandomRemoveConfig())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Replace, BuilderExtensions.RandomBaseConfig<RequestTransformerAdvancedTransformReplace>())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .With(x => x.HttpMethod, BuilderExtensions.RandomHttpMethod())
                    .With(x => x.Rename, BuilderExtensions.RandomBaseConfig<RequestTransformerAdvancedTransformBase>())
                    .Build();

                yield return Builder<TPluginConfig>
                    .CreateNew()
                    .PopulateRequestTransformerConfig()
                    .Build();
            }
        }
    }
}
