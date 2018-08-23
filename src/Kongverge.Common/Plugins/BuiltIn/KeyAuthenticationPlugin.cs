using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class KeyAuthenticationPlugin : KongPluginBase<KeyAuthenticationConfig>
    {
        public KeyAuthenticationPlugin() : base("key-auth")
        {
        }

        protected override KeyAuthenticationConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new KeyAuthenticationConfig
            {
                KeyNames = pluginBody.config.ReadStringSet("key_names"),
                KeyInBody = pluginBody.config.ReadBool("key_in_body"),
                Anonymous = pluginBody.config.ReadString("anonymous"),
                RunOnPreflight = pluginBody.config.ReadBool("run_on_preflight"),
                HideCredentials = pluginBody.config.ReadBool("hide_credentials")
            };
        }

        protected override PluginBody DoCreatePluginBody(KeyAuthenticationConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "key_names", target.KeyNames.ToCommaSeperatedString()},
                { "key_in_body", target.KeyInBody.ToString().ToLower()},
                { "anonymous", target.Anonymous},
                { "run_on_preflight", target.RunOnPreflight.ToString().ToLower()},
                { "hide_credentials", target.HideCredentials.ToString().ToLower()},
            });
        }
    }
}
