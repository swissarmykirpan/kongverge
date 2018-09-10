using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kongverge.Common.Helpers;
using Newtonsoft.Json;

namespace Kongverge.Common.DTOs
{
    public sealed class KongRoute : ExtendibleKongObject, IKongEquatable<KongRoute>
    {
        private int _regexPriority;
        private const int DefaultRegexMultiplier = 10;

        [JsonProperty("service")]
        public ServiceReference Service { get; set; }
        
        [JsonProperty("hosts")]
        public IEnumerable<string> Hosts { get; set; } = new List<string>();

        [JsonProperty("protocols")]
        public IEnumerable<string> Protocols { get; set; } = new List<string>();

        [JsonProperty("methods")]
        public IEnumerable<string> Methods { get; set; } = new List<string>();

        [JsonProperty("paths")]
        public IEnumerable<string> Paths { get; set; } = new List<string>();

        // TODO: This property has a default value. Therefore if it isn't specified in config files, it won't round-trip properly.
        // Consider making this nullable and then checking if the returned value from Kong is the same as the default, and if so, setting it to null when StripPersistedValues() is invoked.
        [JsonProperty("regex_priority")]
        public int RegexPriority
        {
            get { return _regexPriority > 0 ? _regexPriority : Paths.Select(path => path.Count(f => f == '/')).Concat(new[] { 0 }).Max() * DefaultRegexMultiplier; }
            set => _regexPriority = value;
        }

        [JsonProperty("strip_path")]
        public bool StripPath { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Paths: {string.Join(", ", Paths)}";
        }

        public StringContent ToJsonStringContent()
        {
            var serviceReference = Service;
            var plugins = Plugins;

            Service = null;
            Plugins = null;
            var json = JsonConvert.SerializeObject(this, KongJsonConvert.SerializerSettings);
            Service = serviceReference;
            Plugins = plugins;

            return json.AsJsonStringContent();
        }

        public override void StripPersistedValues()
        {
            base.StripPersistedValues();
            Service = null;
        }

        public override void AssignParentId(KongPlugin plugin)
        {
            base.AssignParentId(plugin);
            plugin.RouteId = Id;
        }

        public object[] GetEqualityValues() =>
            new object[]
            {
                Hosts,
                Protocols,
                Methods,
                Paths,
                RegexPriority,
                StripPath
            };

        public bool Equals(KongRoute other) => this.KongEquals(other);

        public override bool Equals(object obj) => this.KongEqualsObject(obj);

        public override int GetHashCode() => this.GetKongHashCode();

        public class ServiceReference
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }
    }
}
