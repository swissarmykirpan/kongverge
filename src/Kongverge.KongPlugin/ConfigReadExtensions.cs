using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Kongverge.KongPlugin
{
    public static class ConfigReadExtensions
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

        public static IDictionary<string, string> ReadStringMaps(this IDictionary<string, object> values, string key)
        {
            var stringValues = values.ReadStrings(key);
            if (values == null)
            {
                return new Dictionary<string, string>();
            }

            return StringsToMaps(stringValues);
        }

        public static IDictionary<string, string> StringsToMaps(IEnumerable<string> values)
        {
            return values
                .Select(value => value.Split(':'))
                .ToDictionary(kv => kv[0], kv => kv.Length > 1 ? kv[1] : string.Empty);
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

            switch (obj)
            {
                case JObject jObject:
                    return jObject.ToObject<Dictionary<string, object>>();

                default:
                    return new Dictionary<string, object>();
            }
        }

        public static string ToCommaSeperatedString(this IEnumerable<string> strings)
        {
            return string.Join(",", strings);
        }

        public static IEnumerable<string> ToCommaSeperatedStrings(this IDictionary<string, string> stringMaps)
        {
            return stringMaps
                .Select(kv => $"{kv.Key}:{kv.Value}");
        }

        public static bool SetsMatch<T>(ICollection<T> a, ICollection<T> b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Count != b.Count)
            {
                return false;
            }

            return !a.Except(b).Concat(b.Except(a)).Any();
        }
    }
}
