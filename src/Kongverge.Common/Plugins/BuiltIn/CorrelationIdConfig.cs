using Kongverge.Extension;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class CorrelationIdConfig : IKongPluginConfig
    {
        public string Header { get; set; }
        public string Template { get; set; }
        public bool EchoDownstream { get; set; }

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is CorrelationIdConfig otherConfig)
            {
                return Header == otherConfig.Header
                       && Template == otherConfig.Template
                       && EchoDownstream == otherConfig.EchoDownstream;
            }

            return false;
        }
    }
}
