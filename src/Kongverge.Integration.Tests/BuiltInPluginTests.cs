using System.Collections.Generic;
using AutoFixture;
using Kongverge.Common.Plugins.BuiltIn;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;
using Kongverge.TestHelpers;

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

        protected override IEnumerable<KeyAuthenticationConfig> Permutations
        {
            get
            {
                var fixture = new Fixture();

                yield return fixture.Create<KeyAuthenticationConfig>();

                yield return fixture.Build<KeyAuthenticationConfig>()
                    .Without(x => x.Anonymous)
                    .Create();
            }
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
                var fixture = new Fixture();

                yield return fixture.Build<TPluginConfig>()
                    .With(x => x.HttpMethod, Random.HttpMethod())
                    .Without(x => x.Add)
                    .Without(x => x.Append)
                    .Without(x => x.Remove)
                    .Without(x => x.Rename)
                    .Create();

                yield return fixture.Build<TPluginConfig>()
                    .With(x => x.HttpMethod, Random.HttpMethod())
                    .Without(x => x.Replace)
                    .Without(x => x.Append)
                    .Without(x => x.Remove)
                    .Without(x => x.Rename)
                    .Create();

                yield return fixture.Build<TPluginConfig>()
                    .With(x => x.HttpMethod, Random.HttpMethod())
                    .Without(x => x.Replace)
                    .Without(x => x.Add)
                    .Without(x => x.Remove)
                    .Without(x => x.Rename)
                    .Create();

                yield return fixture.Build<TPluginConfig>()
                    .With(x => x.HttpMethod, Random.HttpMethod())
                    .Without(x => x.Replace)
                    .Without(x => x.Add)
                    .Without(x => x.Append)
                    .Without(x => x.Rename)
                    .Create();

                yield return fixture.Build<TPluginConfig>()
                    .With(x => x.HttpMethod, Random.HttpMethod())
                    .Without(x => x.Remove)
                    .Without(x => x.Add)
                    .Without(x => x.Append)
                    .Without(x => x.Rename)
                    .Create();

                yield return fixture.Build<TPluginConfig>()
                    .With(x => x.HttpMethod, Random.HttpMethod())
                    .Without(x => x.Remove)
                    .Without(x => x.Add)
                    .Without(x => x.Append)
                    .Without(x => x.Replace)
                    .Create();

                yield return fixture.Build<TPluginConfig>()
                    .With(x => x.HttpMethod, Random.HttpMethod())
                    .Create();
            }
        }
    }
}
