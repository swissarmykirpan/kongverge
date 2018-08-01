using System;

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
            return pluginBody.config.ContainsKey(key) ? Convert.ToInt32(pluginBody.config[key]) : 0;
        }

        public static bool ReadConfigBool(this PluginBody pluginBody, string key)
        {
            if (!pluginBody.config.ContainsKey(key))
            {
                return false;
            }

            var obj = pluginBody.config[key];
            switch (obj)
            {
                case bool b:
                    return b;

                case string s:
                    var parseSuccess = bool.TryParse(s, out var result);
                    if (parseSuccess)
                    {
                        return result;
                    }

                    return false;

                default:
                    return false;
            }
        }

        public static string[] ReadConfigStringArray(this PluginBody pluginBody, string key)
        {
            if (pluginBody.config.ContainsKey(key))
            {
                return ((string) pluginBody.config[key])?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };
            }

            return new string[] { };
        }
    }
}
