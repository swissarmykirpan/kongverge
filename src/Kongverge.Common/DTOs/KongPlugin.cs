using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kongverge.Common.Helpers;
using Newtonsoft.Json;

namespace Kongverge.Common.DTOs
{
    public sealed class KongPlugin : KongObject, IKongEquatable<KongPlugin>
    {
        [JsonProperty("consumer_id")]
        public string ConsumerId { get; set; }

        [JsonProperty("service_id")]
        public string ServiceId { get; set; }

        [JsonProperty("route_id")]
        public string RouteId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("config")]
        public Dictionary<string, object> Config { get; set; }

        public bool IsGlobal() =>
            ConsumerId == null &&
            ServiceId == null &&
            RouteId == null;

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}";
        }

        public StringContent ToJsonStringContent() =>
            JsonConvert.SerializeObject(this, KongJsonConvert.SerializerSettings).AsJsonStringContent();

        public override void StripPersistedValues()
        {
            base.StripPersistedValues();
            ConsumerId = null;
            ServiceId = null;
            RouteId = null;
        }

        public override bool IsMatch<T>(T other)
        {
            return other is KongPlugin kongPlugin && kongPlugin.Name == Name;
        }

        public object[] GetEqualityValues() =>
             new object[]
             {
                Name,
                Config
             };

        public bool Equals(KongPlugin other) => this.KongEquals(other);

        public override bool Equals(object obj) => this.KongEqualsObject(obj);

        public override int GetHashCode() => this.GetKongHashCode();
    }
}
