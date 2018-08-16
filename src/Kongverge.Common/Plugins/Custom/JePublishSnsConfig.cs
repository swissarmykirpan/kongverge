using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.Custom
{
    public class JePublishSnsConfig : IKongPluginConfig
    {
        public int Timeout { get; set; }

        public int KeepAlive { get; set; }

        [JsonProperty("aws_key")]
        public string AwsKey { get; set; }

        [JsonProperty("aws_secret")]
        public string AwsSecret { get; set; }

        [JsonProperty("aws_region")]
        public string AwsRegion { get; set; }

        [JsonProperty("account_id")]
        public int AccountId { get; set; }

        [JsonProperty("consul_url")]
        public string ConsulUrl { get; set; }

        [JsonProperty("topic_name")]
        public string TopicName { get; set; }

        public string Environment { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is JePublishSnsConfig otherConfig)
            {
                return
                    (Timeout == otherConfig.Timeout) &&
                    (KeepAlive == otherConfig.KeepAlive) &&
                    (AwsKey == otherConfig.AwsKey) &&
                    (AwsSecret == otherConfig.AwsSecret) &&
                    (AwsRegion == otherConfig.AwsRegion) &&
                    (AccountId == otherConfig.AccountId) &&
                    (ConsulUrl == otherConfig.ConsulUrl) &&
                    (TopicName == otherConfig.TopicName) &&
                    (Environment == otherConfig.Environment);
            }

            return false;
        }

        public string id { get; set; }
    }
}
