using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Kongverge.KongPlugin
{
    public static class ConfigWriteExtensions
    {
        public static string ToCommaSeperatedString(this IEnumerable<string> strings)
        {
            return string.Join(",", strings);
        }

        public static IEnumerable<string> ToCommaSeperatedStrings(this IDictionary<string, string> stringMaps)
        {
            return stringMaps
                .Select(kv => $"{kv.Key}:{kv.Value}");
        }

        public static string ToJsonString<T>(this T value) where T : Enum
        {
            var attributes = (DescriptionAttribute[])typeof(T).GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0
                ? attributes[0].Description
                : value.ToString().ToLowerInvariant();
        }

        public static JArray ToJArray<T>(this IEnumerable<T> values)
        {
            return new JArray(values.Select(x => x.ToJObject()));
        }

        public static JObject ToJObject<T>(this T value)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>(new[] { new StringEnumConverter() })
            };
            var serialized = JsonConvert.SerializeObject(value, serializerSettings);
            return JObject.FromObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(serialized));
        }
    }
}
