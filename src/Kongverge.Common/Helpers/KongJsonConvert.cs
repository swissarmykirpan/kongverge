using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kongverge.Common.Helpers
{
    public static class KongJsonConvert
    {
        private static readonly JsonSerializerSettings KongSerializerSettings =
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>(new[] { new StringEnumConverter() })
            };

        public static StringContent Serialize<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data, KongSerializerSettings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
