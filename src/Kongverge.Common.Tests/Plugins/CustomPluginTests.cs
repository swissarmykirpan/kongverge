using FizzWare.NBuilder;
using Kongverge.Common.Plugins.Custom;

namespace Kongverge.Common.Tests.Plugins
{
    public class JeJustSayingDefaultsPluginTests : PluginTests<JeJustSayingDefaultsPlugin, JeJustSayingDefaultsConfig>
    {
    }

    public class JePublishSnsPluginTests : PluginTests<JePublishSnsPlugin, JePublishSnsConfig>
    {
    }

    public class JeRequestDelayPluginTests : PluginTests<JeRequestDelayPlugin, JeRequestDelayConfig>
    {
    }

    public class JeTcpLogPluginTests : PluginTests<JeTcpLogPlugin, JeTcpLogConfig>
    {
    }

    public class JeUdpLogPluginTests : PluginTests<JeUdpLogPlugin, JeUdpLogConfig>
    {
    }

    public class JeRequestTransformerPluginTests : PluginTests<JeRequestTransformerPlugin, JeRequestTransformerConfig>
    {
        protected override ISingleObjectBuilder<JeRequestTransformerConfig> Populate(ISingleObjectBuilder<JeRequestTransformerConfig> builder)
        {
            return builder.PopulateRequestTransformerConfig();
        }
    }
}
