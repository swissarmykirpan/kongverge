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
            if (pluginBody.config.ContainsKey(key))
            {
                return (int)pluginBody.config[key];
            }

            return 0;
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
                case bool _:
                    return (bool)obj;

                case string _:
                    var parseSuccess = bool.TryParse(obj as string, out var result);
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
