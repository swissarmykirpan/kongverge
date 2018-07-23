using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Kongverge.Common.Helpers
{
    public static class WriteToKong
    {
        private static readonly JsonSerializerSettings Settings =
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

        public static StringContent SerializeObject<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data, Settings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
