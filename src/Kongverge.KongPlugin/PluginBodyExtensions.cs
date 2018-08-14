using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Kongverge.KongPlugin
{
    public static class PluginBodyExtensions
    {
        public static string ReadConfigString(this PluginBody pluginBody, string key)
        {
            return pluginBody.config.ReadString(key);
        }

        public static string[] ReadConfigStrings(this PluginBody pluginBody, string key)
        {
            return pluginBody.config.ReadStrings(key);
        }

        public static int ReadConfigInt(this PluginBody pluginBody, string key)
        {
            return pluginBody.config.ReadInt(key);
        }

        public static int[] ReadConfigInts(this PluginBody pluginBody, string key)
        {
            return pluginBody.config.ReadInts(key);
        }

        public static long ReadConfigLong(this PluginBody pluginBody, string key)
        {
            return pluginBody.config.ReadLong(key);
        }

        public static bool ReadConfigBool(this PluginBody pluginBody, string key)
        {
            return pluginBody.config.ReadBool(key);
        }
    }

    public static class ObjectDictionaryExtensions
    {
        public static int ReadInt(this IDictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key))
            {
                return 0;
            }

            var obj = values[key];

            switch (obj)
            {
                case long lValue:
                    return (int)lValue;

                case int value:
                    return value;

                default:
                    return 0;
            }
        }

        public static long ReadLong(this IDictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key))
            {
                return 0L;
            }

            return (long)values[key];
        }

        public static bool ReadBool(this IDictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key))
            {
                return false;
            }

            var obj = values[key];
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

        public static string ReadString(this IDictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key))
            {
                return string.Empty;
            }

            return values[key]?.ToString();
        }


        public static string[] ReadStrings(this IDictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key))
            {
                return new string[0];
            }

            var obj = values[key];

            switch (obj)
            {
                case string[] arrayValues:
                    return arrayValues;

                case JArray jArray:
                    return jArray.Select(v => v.ToString()).ToArray();

                case string value:
                    return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                default:
                    return new string[0];
            }
        }

        public static HashSet<string> ReadStringSet(this IDictionary<string, object> values, string key)
        {
            var stringValues = values.ReadStrings(key);
            if (values == null)
            {
                return new HashSet<string>();
            }

            return new HashSet<string>(stringValues);
        }

        public static int[] ReadInts(this IDictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key))
            {
                return new int[0];
            }

            var obj = values[key];

            switch (obj)
            {
                case int[] ints:
                    return ints;

                case JArray jArray:
                    return jArray.Select(v => (int)v).ToArray();

                case int value:
                    return new[] { value };

                default:
                    return new int[0];
            }
        }

        public static IDictionary<string, object> SubProperties(this IDictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key))
            {
                return new Dictionary<string, object>();
            }

            var obj = values[key];
            if (!(obj is JObject))
            {
                return new Dictionary<string, object>();

            }

            return (obj as JObject).ToObject<Dictionary<string, object>>();
        }
    }
}
