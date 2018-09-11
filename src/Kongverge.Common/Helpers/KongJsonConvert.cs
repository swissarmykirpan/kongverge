using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using Kongverge.Common.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kongverge.Common.Helpers
{
    public static class KongJsonConvert
    {
        public static readonly JsonSerializerSettings SerializerSettings =
            new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

        public static StringContent AsJsonStringContent(this string json) =>
            new StringContent(json, Encoding.UTF8, "application/json");

        public static T ToKongObject<T>(this string json) where T : ExtendibleKongObject =>
            JsonConvert.DeserializeObject<T>(json, SerializerSettings);

        public static string ToConfigJson(this ExtendibleKongObject kongObject)
        {
            kongObject.StripPersistedValues();
            return SerializeObject(kongObject) + Environment.NewLine;
        }

        public static string SerializeObject(object value)
        {
            var token = JToken.FromObject(value, JsonSerializer.Create(SerializerSettings));
            return JsonConvert.SerializeObject(token.Normalize(), SerializerSettings);
        }

        private static JToken Normalize(this JToken token)
        {
            switch (token)
            {
                case JObject jObject:
                {
                    var normalized = new JObject();
                    foreach (var property in jObject.Properties().OrderBy(x => x.Name))
                    {
                        if (property.Value is JArray jArray && jArray.Count == 0)
                        {
                            continue;
                        }
                        normalized.Add(property.Name, property.Value.Normalize());
                    }
                    return normalized;
                }
                case JArray jArray:
                {
                    if (jArray.Count > 0 && jArray.All(x => x is JValue))
                    {
                        var firstType = jArray[0].Type;
                        if (jArray.Select(x => x.Type).All(x => x == firstType))
                        {
                            return new JArray(jArray.OrderBy(x => x));
                        }
                    }
                    for (var i = 0; i < jArray.Count; i++)
                    {
                        jArray[i] = jArray[i].Normalize();
                    }
                    return jArray;
                }
                default:
                    return token;
            }
        }
    }
}
