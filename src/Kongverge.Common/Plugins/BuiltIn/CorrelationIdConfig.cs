using System.ComponentModel;
using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public enum CorrelationIdGenerator
    {
        // ReSharper disable InconsistentNaming
        // This is the default, so it comes first
        [Description("uuid#counter")]
        uuidCounter,
        uuid,
        tracker
        // ReSharper restore InconsistentNaming
    }

    public class CorrelationIdConfig : IKongPluginConfig
    {
        [JsonProperty("header_name")]
        public string HeaderName { get; set; }
        [JsonProperty("generator")]
        public CorrelationIdGenerator Generator { get; set; }
        public bool EchoDownstream { get; set; }

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is CorrelationIdConfig otherConfig)
            {
                return HeaderName == otherConfig.HeaderName &&
                       Generator == otherConfig.Generator &&
                       EchoDownstream == otherConfig.EchoDownstream;
            }

            return false;
        }
    }
}
