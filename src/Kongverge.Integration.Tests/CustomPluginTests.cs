using Kongverge.Common.Plugins.Custom;

namespace Kongverge.Integration.Tests
{
    public class JeJustSayingDefaultsPluginTests : PluginTests<JeJustSayingDefaultsConfig>
    {
        public JeJustSayingDefaultsPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }

    public class JePublishSnsPluginTests : PluginTests<JePublishSnsConfig>
    {
        public JePublishSnsPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }

    public class JeRequestDelayPluginTests : PluginTests<JeRequestDelayConfig>
    {
        public JeRequestDelayPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }

    public class JeTcpLogPluginTests : PluginTests<JeTcpLogConfig>
    {
        public JeTcpLogPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }
    
    public class JeUdpLogPluginTests : PluginTests<JeUdpLogConfig>
    {
        public JeUdpLogPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }
    
    public class JeRequestTransformerPluginTests : RequestTransformerPluginTests<JeRequestTransformerConfig>
    {
        public JeRequestTransformerPluginTests(KongvergeTestFixture kongvergeTestFixture) : base(kongvergeTestFixture)
        {
        }
    }
}
