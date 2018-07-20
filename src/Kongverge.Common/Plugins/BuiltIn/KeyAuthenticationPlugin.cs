using System.Collections.Generic;
using Kongverge.Extension;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class KeyAuthenticationPlugin : KongPluginBase<KeyAuthenticationConfig>
    {
        public KeyAuthenticationPlugin() : base("key-auth")
        {
        }

        public override string PluginName => "key-auth";

        public override KeyAuthenticationConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new KeyAuthenticationConfig
            {
                KeyNames = new HashSet<string>(((string)pluginBody.config["key_names"]).Split(',')),
                KeyInBody = (bool)pluginBody.config["key_in_body"],
                Anonymous = (string) pluginBody.config["anonymous"],
                RunOnPreflight = (bool) pluginBody.config["run_on_preflight"],
                HideCredentials = (bool) pluginBody.config["hide_credentials"],
            };
        }

        public override PluginBody DoCreatePluginBody(KeyAuthenticationConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "key_names", string.Join(',', target.KeyNames)},
                { "key_in_body", target.KeyInBody.ToString().ToLower()},
                { "anonymous", target.Anonymous},
                { "run_on_preflight", target.RunOnPreflight.ToString().ToLower()},
                { "hide_credentials", target.HideCredentials.ToString().ToLower()},
            });
        }
    }
}
