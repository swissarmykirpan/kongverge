using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class KeyAuthenticationPlugin : KongPluginBase<KeyAuthenticationConfig>
    {
        public KeyAuthenticationPlugin() : base("key-auth")
        {
        }

        public override string[] PluginNames => new[]{"key-auth"};

        protected override KeyAuthenticationConfig DoCreateConfigObject(PluginBody pluginBody)
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

        protected override PluginBody DoCreatePluginBody(KeyAuthenticationConfig target)
        {
            return new PluginBody(PluginNames[0], new Dictionary<string, object>
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
