using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.DTOs
{
    public sealed class KongService : ExtendibleKongObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("validate-host")]
        public bool ValidateHost { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; } = 80;

        [JsonProperty("protocol")]
        public string Protocol { get; set; } = "http";

        [JsonProperty("retries")]
        public int Retries { get; set; } = 5;

        [JsonProperty("connect_timeout")]
        public int ConnectTimeout { get; set; } = 300;

        [JsonProperty("write_timeout")]
        public int WriteTimeout { get; set; } = 100;

        [JsonProperty("read_timeout")]
        public int ReadTimeout { get; set; } = 1500;

        [JsonProperty("path")]
        public string Path { get; set; }

        public IReadOnlyCollection<KongRoute> Routes { get; set; } = Array.Empty<KongRoute>();

        public bool Equals(KongService other)
        {
            if (other == null)
            {
                return false;
            }

            return
                string.Equals(Name, other.Name)
                && string.Equals(Host, other.Host)
                && Port == other.Port
                && string.Equals(Protocol, other.Protocol)
                && Retries == other.Retries
                && ConnectTimeout == other.ConnectTimeout
                && WriteTimeout == other.WriteTimeout
                && ReadTimeout == other.ReadTimeout
                && string.Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
#pragma warning disable IDE0041 // Use 'is null' check
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals(obj as KongService);
#pragma warning restore IDE0041 // Use 'is null' check
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var nameBytes = Encoding.ASCII.GetBytes(Name);
                return nameBytes.Aggregate(0, (current, nameChar) => (current * 397) ^ nameChar);
            }
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}";
        }

        protected override PluginBody DoDecoratePluginBody(PluginBody body)
        {
            body.service_id = Id;

            return body;
        }

        public bool ShouldSerializeValidateHost()
        {
            return false;   
        }
    }
}
