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

        public Template ParseTemplate(object text)
        {
            switch (text.ToString())
            {
                case "uuid":
                    return Template.UUID;
                case "tracker":
                    return Template.Tracker;
                case "uuid#counter": //Cursed #
                    return Template.Counter;
                default:
                    Log.Error("Invalid value for template: {text}", text);
                    throw new InvalidOperationException("Invalid value for template");
            }
        }

        public string SerializeTemplate(Template templ)
        {
            switch (templ)
            {
                case Template.UUID:
                    return "uuid";
                case Template.Tracker:
                    return "tracker";
                case Template.Counter:
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
                Template = ParseTemplate(pluginBody.ReadConfigString("generator")),
                EchoDownstream = pluginBody.ReadConfigBool("echo_downstream"),
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
