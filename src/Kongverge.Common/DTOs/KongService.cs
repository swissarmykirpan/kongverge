using System;
using System.Collections.Generic;
using System.Net.Http;
using Kongverge.Common.Helpers;
using Newtonsoft.Json;

namespace Kongverge.Common.DTOs
{
    public sealed class KongService : ExtendibleKongObject, IKongEquatable<KongService>
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        // TODO: This is not a Kong property. Therefore if it is specified in config files, it won't round-trip properly.
        // Consider implementing this functionality another way (perhaps as part of service tests).
        [JsonProperty("validate-host")]
        public bool? ValidateHost { get; set; }

        // TODO: This property has a default value. Therefore if it isn't specified in config files, it won't round-trip properly.
        // Consider making this nullable and then checking if the returned value from Kong is the same as the default, and if so, setting it to null when StripPersistedValues() is invoked.
        [JsonProperty("port")]
        public int Port { get; set; } = 80;

        [JsonProperty("protocol")]
        public string Protocol { get; set; } = "http";

        // TODO: This property has a default value. Therefore if it isn't specified in config files, it won't round-trip properly.
        // Consider making this nullable and then checking if the returned value from Kong is the same as the default, and if so, setting it to null when StripPersistedValues() is invoked.
        [JsonProperty("retries")]
        public int Retries { get; set; } = 5;

        // TODO: This property has a default value. Therefore if it isn't specified in config files, it won't round-trip properly.
        // Consider making this nullable and then checking if the returned value from Kong is the same as the default, and if so, setting it to null when StripPersistedValues() is invoked.
        [JsonProperty("connect_timeout")]
        public int ConnectTimeout { get; set; } = 300;

        // TODO: This property has a default value. Therefore if it isn't specified in config files, it won't round-trip properly.
        // Consider making this nullable and then checking if the returned value from Kong is the same as the default, and if so, setting it to null when StripPersistedValues() is invoked.
        [JsonProperty("write_timeout")]
        public int WriteTimeout { get; set; } = 100;

        // TODO: This property has a default value. Therefore if it isn't specified in config files, it won't round-trip properly.
        // Consider making this nullable and then checking if the returned value from Kong is the same as the default, and if so, setting it to null when StripPersistedValues() is invoked.
        [JsonProperty("read_timeout")]
        public int ReadTimeout { get; set; } = 1500;

        [JsonProperty("path")]
        public string Path { get; set; }
        
        [JsonProperty("routes")]
        public IReadOnlyList<KongRoute> Routes { get; set; } = Array.Empty<KongRoute>();

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}";
        }

        public StringContent ToJsonStringContent()
        {
            var validateHost = ValidateHost;
            var routes = Routes;
            var plugins = Plugins;

            ValidateHost = null;
            Routes = null;
            Plugins = null;
            var json = JsonConvert.SerializeObject(this, KongJsonConvert.SerializerSettings);
            ValidateHost = validateHost;
            Routes = routes;
            Plugins = plugins;

            return json.AsJsonStringContent();
        }

        public override void StripPersistedValues()
        {
            base.StripPersistedValues();
            foreach (var route in Routes)
            {
                route.StripPersistedValues();
            }
        }

        public override void AssignParentId(KongPlugin plugin)
        {
            base.AssignParentId(plugin);
            plugin.ServiceId = Id;
        }

        public object[] GetEqualityValues() =>
            new object[]
            {
                Name,
                Host,
                Port,
                Protocol,
                Retries,
                ConnectTimeout,
                WriteTimeout,
                ReadTimeout,
                Path
            };

        public bool Equals(KongService other) => this.KongEquals(other);

        public override bool Equals(object obj) => this.KongEqualsObject(obj);

        public override int GetHashCode() => this.GetKongHashCode();
    }
}
