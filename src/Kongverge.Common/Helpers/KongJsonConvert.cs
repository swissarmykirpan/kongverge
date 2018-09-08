using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Kongverge.Common.DTOs;
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

        public static StringContent ToJsonStringContent(this KongService kongService)
        {
            var validateHost = kongService.ValidateHost;
            var routes = kongService.Routes;
            var plugins = kongService.Plugins;

            kongService.ValidateHost = null;
            kongService.Routes = null;
            kongService.Plugins = null;
            var json = JsonConvert.SerializeObject(kongService, KongSerializerSettings);
            kongService.ValidateHost = validateHost;
            kongService.Routes = routes;
            kongService.Plugins = plugins;

            return json.ToJsonStringContent();
        }

        public static StringContent ToJsonStringContent(this KongRoute kongRoute)
        {
            var serviceReference = kongRoute.Service;
            var plugins = kongRoute.Plugins;

            kongRoute.Service = null;
            kongRoute.Plugins = null;
            var json = JsonConvert.SerializeObject(kongRoute, KongSerializerSettings);
            kongRoute.Service = serviceReference;
            kongRoute.Plugins = plugins;

            return json.ToJsonStringContent();
        }

        public static StringContent ToJsonStringContent(this KongPlugin kongPlugin) =>
            JsonConvert.SerializeObject(kongPlugin, KongSerializerSettings).ToJsonStringContent();

        private static StringContent ToJsonStringContent(this string json) =>
            new StringContent(json, Encoding.UTF8, "application/json");
    }
}
