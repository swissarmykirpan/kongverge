using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.Custom
{
    public class PublishSnsPlugin : KongPluginBase<PublishSnsConfig>
    {
        public PublishSnsPlugin() : base("publish-sns")
        {
        }

        public override string PluginName => "je-publish-sns";

        protected  override PublishSnsConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new PublishSnsConfig
            {
                Timeout = pluginBody.ReadConfigInt("timeout"),
                KeepAlive = pluginBody.ReadConfigInt("keepalive"),
                AwsKey = pluginBody.ReadConfigString("aws_key"),
                AwsSecret = pluginBody.ReadConfigString("aws_secret"),
                AwsRegion = pluginBody.ReadConfigString("aws_region"),
                AccountId = pluginBody.ReadConfigInt("account_id"),
                ConsulUrl = pluginBody.ReadConfigString("consul_url"),
                TopicName = pluginBody.ReadConfigString("topic_name"),
                Environment = pluginBody.ReadConfigString("environment")
            };
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
