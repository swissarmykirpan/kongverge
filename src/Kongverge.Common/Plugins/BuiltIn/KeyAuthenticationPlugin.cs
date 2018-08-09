using System.Collections.Generic;
using System.Linq;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class KeyAuthenticationPlugin : KongPluginBase<KeyAuthenticationConfig>
    {
        public KeyAuthenticationPlugin() : base("key-auth")
        {
        }

        public override string PluginName => "key-auth";

        protected override KeyAuthenticationConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new KeyAuthenticationConfig
            {
                KeyNames = pluginBody.ReadConfigStrings("key_names").ToHashSet(),
                KeyInBody = pluginBody.ReadConfigBool("key_in_body"),
                Anonymous = pluginBody.ReadConfigString("anonymous"),
                RunOnPreflight = pluginBody.ReadConfigBool("run_on_preflight"),
                HideCredentials = pluginBody.ReadConfigBool("hide_credentials")
            };
        }

        protected override PluginBody DoCreatePluginBody(KeyAuthenticationConfig target)
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
