using System;
using System.Collections.Generic;
using Kongverge.KongPlugin;
using Serilog;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class CorrelationIdPlugin : KongPluginBase<CorrelationIdConfig>
    {
        public CorrelationIdPlugin() : base("correlation")
        {
        }

        public override string PluginName => "correlation-id";

        public CorrelationIdGenerator ParseTemplate(object text)
        {
            switch (text.ToString())
            {
                case "uuid":
                    return CorrelationIdGenerator.UUID;
                case "tracker":
                    return CorrelationIdGenerator.Tracker;
                case "uuid#counter": //Cursed #
                    return CorrelationIdGenerator.Counter;
                default:
                    Log.Error("Invalid value for template: {text}", text);
                    throw new InvalidOperationException("Invalid value for template");
            }
        }

        public string SerializeTemplate(CorrelationIdGenerator templ)
        {
            switch (templ)
            {
                case CorrelationIdGenerator.UUID:
                    return "uuid";
                case CorrelationIdGenerator.Tracker:
                    return "tracker";
                case CorrelationIdGenerator.Counter:
                    return "uuid#counter"; //Cursed #
                default:
                    Log.Error("Unable to write json for template value: {value}", templ);
                    throw new InvalidOperationException("Invalid value for template");
            }
        }

        protected override CorrelationIdConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new CorrelationIdConfig
            {
                EchoDownstream = (bool)pluginBody.config["echo_downstream"],
                Template = ParseTemplate(pluginBody.ReadConfigString("generator")),
                Header = pluginBody.ReadConfigString("header_name")
            };
        }

        protected override PluginBody DoCreatePluginBody(CorrelationIdConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "echo_downstream", target.EchoDownstream },
                { "generator", SerializeTemplate(target.Template) },
                { "header_name", target.Header }
            });
        }
    }
}
