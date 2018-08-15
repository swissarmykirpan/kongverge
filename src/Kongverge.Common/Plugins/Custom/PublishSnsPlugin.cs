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
                Timeout = pluginBody.config.ReadInt("timeout"),
                KeepAlive = pluginBody.config.ReadInt("keepalive"),
                AwsKey = pluginBody.config.ReadString("aws_key"),
                AwsSecret = pluginBody.config.ReadString("aws_secret"),
                AwsRegion = pluginBody.config.ReadString("aws_region"),
                AccountId = pluginBody.config.ReadInt("account_id"),
                ConsulUrl = pluginBody.config.ReadString("consul_url"),
                TopicName = pluginBody.config.ReadString("topic_name"),
                Environment = pluginBody.config.ReadString("environment")
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
