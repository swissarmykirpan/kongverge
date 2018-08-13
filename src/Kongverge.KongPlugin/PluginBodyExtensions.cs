using System;
using System.Linq;
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

        public static string[] ReadConfigStrings(this PluginBody pluginBody, string key)
        {
            if (pluginBody.config.ContainsKey(key))
            {
                var obj = pluginBody.config[key];

                switch (obj)
                {
                    case string[] values:
                        return values;

                    case JArray jArray:
                        return jArray.Select(v => v.ToString()).ToArray();

                    case string value:
                        return value.Split(',').ToArray();

                    default:
                        return new string[0];
                }
            }

            return new string[0];
        }


        public static int ReadConfigInt(this PluginBody pluginBody, string key)
        {
            if (pluginBody.config.ContainsKey(key))
            {
                var obj = pluginBody.config[key];

                switch (obj)
                {
                    case long lValue:
                        return (int) lValue;

                    case int value:
                        return value;

                    default:
                        return 0;
                }
            }

            return 0;
        }

        public static int[] ReadConfigInts(this PluginBody pluginBody, string key)
        {
            if (pluginBody.config.ContainsKey(key))
            {
                var obj = pluginBody.config[key];

                switch (obj)
                {
                    case int[] ints:
                        return ints;

                    case JArray jArray:
                        return jArray.Select(v => (int) v).ToArray();

                    case int value:
                        return new [] { value };

                    default:
                        return new int[0];
                }
            }

            return new int[0];
        }

        public static long ReadConfigLong(this PluginBody pluginBody, string key)
        {
            if (pluginBody.config.ContainsKey(key))
            {
                return (long)pluginBody.config[key];
            }

            return 0L;
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
