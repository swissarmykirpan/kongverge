using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.Custom
{
    public class PublishSnsPlugin : KongPluginBase<PublishSnsConfig>
    {
        public PublishSnsPlugin() : base("publish-sns")
        {
        }

        public override string PluginName => "publish-sns";

        protected  override PublishSnsConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new PublishSnsConfig
            {
                Timeout = ReadConfigInt(pluginBody.config,"timeout"),
                KeepAlive = ReadConfigInt(pluginBody.config, "keepalive"),
                AwsKey = ReadConfigString(pluginBody.config, "aws_key"),
                AwsSecret = ReadConfigString(pluginBody.config, "aws_secret"),
                AwsRegion = ReadConfigString(pluginBody.config, "aws_region"),
                AccountId = ReadConfigInt(pluginBody.config,"account_id"),
                ConsulUrl = ReadConfigString(pluginBody.config, "consul_url"),
                TopicName = ReadConfigString(pluginBody.config,"topic_name"),
                Environment = ReadConfigString(pluginBody.config,"environment")
            };
        }

        private string ReadConfigString(IDictionary<string, object> config, string key)
        {
            if (config.ContainsKey(key))
            {
                return config[key]?.ToString();
            }

            return string.Empty;
        }

        private int ReadConfigInt(IDictionary<string, object> config, string key)
        {
            if (config.ContainsKey(key))
            {
                return (int)config[key];
            }

            return 0;
        }

        protected override PluginBody DoCreatePluginBody(PublishSnsConfig target)
        {
            var config = new Dictionary<string, object>
            {
                { "timeout", target.Timeout },
                { "keepalive", target.KeepAlive },
                { "aws_key", target.AwsKey },
                { "aws_secret", target.AwsSecret },
                { "aws_region", target.AwsRegion },
                { "account_id", target.AccountId },
                { "consul_url", target.ConsulUrl },
                { "topic_name", target.TopicName },
                { "environment", target.Environment }
            };

            return new PluginBody(PluginName, config);
        }
    }
}
