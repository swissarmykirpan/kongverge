using Newtonsoft.Json.Linq;

namespace Kongverge.KongPlugin
{
    public static class PluginBodyExtensions
    {
        public static string ReadConfigString(this PluginBody pluginBody, string key)
        {
            if (pluginBody.config.ContainsKey(key))
            {
                return pluginBody.config[key]?.ToString();
            }

            return string.Empty;
        }

        public static int ReadConfigInt(this PluginBody pluginBody, string key)
        {
            if (pluginBody.config.ContainsKey(key))
            {
                return (int)pluginBody.config[key];
            }

            return 0;
        }

        public static string[] ExtractArrayFromBody(this PluginBody pluginBody, string configKey, string childKey)
        {
            return ((JObject)pluginBody.config[configKey]).SafeCastJObjectProperty<string[]>(childKey) ?? new string[] { };
        }
    }
}
