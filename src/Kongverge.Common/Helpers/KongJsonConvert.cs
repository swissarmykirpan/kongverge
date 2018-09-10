using System.Net.Http;
using System.Text;
using Kongverge.Common.DTOs;
using Newtonsoft.Json;

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
            return JsonConvert.SerializeObject(kongObject, SerializerSettings);
        }
    }
}
