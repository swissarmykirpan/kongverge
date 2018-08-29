using System.Collections.Generic;
using System.Linq;
using Kongverge.Common.Helpers;
using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.DTOs
{
    public sealed class KongRoute : ExtendibleKongObject
    {
        private int _regexPriority;
        private const int DefaultRegexMultiplier = 10;

        [JsonProperty("hosts")]
        public IEnumerable<string> Hosts { get; set; } = new List<string>();

        [JsonProperty("protocols")]
        public IEnumerable<string> Protocols { get; set; } = new List<string>();

        [JsonProperty("methods")]
        public IEnumerable<string> Methods { get; set; } = new List<string>();

        [JsonProperty("paths")]
        public IEnumerable<string> Paths { get; set; } = new List<string>();

        [JsonProperty("regex_priority")]
        public int RegexPriority
        {
            get { return _regexPriority > 0 ? _regexPriority : Paths.Select(path => path.Count(f => f == '/')).Concat(new[] { 0 }).Max() * DefaultRegexMultiplier; }
            set => _regexPriority = value;
        }

        [JsonProperty("strip_path")]
        public bool StripPath { get; set; }

        public bool Equals(KongRoute other)
        {
            var result = Protocols.SafeEquivalent(other.Protocols)
                          && Hosts.SafeEquivalent(other.Hosts)
                          && Methods.SafeEquivalent(other.Methods)
                          && Paths.SafeEquivalent(other.Paths)
                          && RegexPriority == other.RegexPriority
                          && StripPath == other.StripPath;
            return result;
        }

        public override bool Equals(object obj)
        {
#pragma warning disable IDE0041 // Use 'is null' check
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is KongRoute)) return false;
            return Equals((KongRoute)obj);
#pragma warning restore IDE0041 // Use 'is null' check
        }

        public override int GetHashCode()
        {
            unchecked
            {
#pragma warning disable RCS1212 // Remove redundant assignment.
                var hashCode = Protocols.SequenceHash();
                hashCode = (hashCode * 397) ^ Hosts.SequenceHash();
                hashCode = (hashCode * 397) ^ Methods.SequenceHash();
                hashCode = (hashCode * 397) ^ Paths.SequenceHash();
                hashCode = (hashCode * 397) ^ RegexPriority;
                hashCode = (hashCode * 397) ^ StripPath.GetHashCode();
#pragma warning restore RCS1212 // Remove redundant assignment.
                return hashCode;
            }
        }

        protected override PluginBody DoDecoratePluginBody(PluginBody body)
        {
            body.route_id = Id;
            return body;
        }
    }
}
