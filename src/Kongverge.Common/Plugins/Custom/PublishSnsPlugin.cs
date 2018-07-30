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
                Timeout = (int)pluginBody.config["timeout"],
                KeepAlive = (int)pluginBody.config["keepalive"],
                AwsKey = pluginBody.config["aws_key"].ToString(),
                AwsSecret = pluginBody.config["aws_secret"].ToString(),
                AwsRegion = pluginBody.config["aws_region"].ToString(),
                AccountId = (int)pluginBody.config["account_id"],
                ConsulUrl = pluginBody.config["consul_url"].ToString(),
                TopicName = pluginBody.config["topic_name"].ToString(),
                Environment = pluginBody.config["environment"].ToString()
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
