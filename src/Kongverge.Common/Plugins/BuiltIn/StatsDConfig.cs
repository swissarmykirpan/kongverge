using System.Collections.Generic;
using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class StatsDConfig : IKongPluginConfig
    {
        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("metrics")]
        public HashSet<StatsDMetricConfig> Metrics { get; set; } = new HashSet<StatsDMetricConfig>();

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is StatsDConfig otherConfig)
            {
                return Host == otherConfig.Host &&
                       Port == otherConfig.Port &&
                       ConfigReadExtensions.SetsMatch(Metrics, otherConfig.Metrics) &&
                       Prefix == otherConfig.Prefix;
            }

            return false;
        }
    }

    public struct StatsDMetricConfig
    {
        [JsonProperty("name")]
        public StatsDMetricName Name { get; set; }

        [JsonProperty("stat_type")]
        public StatsDStatType StatType { get; set; }

        [JsonProperty("sample_rate")]
        public int SampleRate { get; set; }

        [JsonProperty("customer_identifier")]
        public CustomerIdentifier CustomerIdentifier { get; set; }
    }

    public enum StatsDMetricName
    {
        // ReSharper disable InconsistentNaming
        request_count,
        request_size,
        response_size,
        latency,
        status_count,
        unique_users,
        request_per_user,
        upstream_latency,
        kong_latency,
        status_count_per_user
        // ReSharper restore InconsistentNaming
    }

    public enum StatsDStatType
    {
        // ReSharper disable InconsistentNaming
        gauge,
        timer,
        counter,
        histogram,
        meter,
        set
        // ReSharper restore InconsistentNaming
    }

    public enum CustomerIdentifier
    {
        // ReSharper disable InconsistentNaming
        consumer_id,
        custom_id,
        username
        // ReSharper restore InconsistentNaming
    }
}
