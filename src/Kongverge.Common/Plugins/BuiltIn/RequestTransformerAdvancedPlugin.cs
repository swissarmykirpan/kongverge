using System.Collections.Generic;
using System.Linq;
using Kongverge.Common.Helpers;
using Kongverge.KongPlugin;
using Newtonsoft.Json.Linq;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTransformerAdvancedPlugin : KongPluginBase<RequestTransformerAdvancedConfig>
    {
        public RequestTransformerAdvancedPlugin() : base("request-transformer-advanced")
        {
        }

        public override string PluginName => "request-transformer-advanced";

        public override RequestTransformerAdvancedConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new RequestTransformerAdvancedConfig
            {
                HttpMethod = (string) pluginBody.config["http_method"],

                Remove = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(ExtractArrayFromBody(pluginBody, "remove", "headers")),
                    QueryString = new HashSet<string>(ExtractArrayFromBody(pluginBody, "remove", "querystring")),
                    Body = new HashSet<string>(ExtractArrayFromBody(pluginBody, "remove", "body"))
                },

                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Headers = new HashSet<string>(ExtractArrayFromBody(pluginBody, "replace", "headers")),
                    QueryString = new HashSet<string>(ExtractArrayFromBody(pluginBody, "replace", "querystring")),
                    Body = new HashSet<string>(ExtractArrayFromBody(pluginBody, "replace", "body")),
                    Uri = ((JObject)pluginBody.config["replace"]).SafeCastJObjectProperty<string>("uri")
                },

                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(ExtractArrayFromBody(pluginBody, "rename", "headers")),
                    QueryString = new HashSet<string>(ExtractArrayFromBody(pluginBody, "rename", "querystring")),
                    Body = new HashSet<string>(ExtractArrayFromBody(pluginBody, "rename", "body"))
                },

                Add = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(ExtractArrayFromBody(pluginBody, "add", "headers")),
                    QueryString = new HashSet<string>(ExtractArrayFromBody(pluginBody, "add", "querystring")),
                    Body = new HashSet<string>(ExtractArrayFromBody(pluginBody, "add", "body"))
                },

                Append = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(ExtractArrayFromBody(pluginBody, "append", "headers")),
                    QueryString = new HashSet<string>(ExtractArrayFromBody(pluginBody, "append", "querystring")),
                    Body = new HashSet<string>(ExtractArrayFromBody(pluginBody, "append", "body"))
                },
            };
        }

        private static string[] ExtractArrayFromBody(PluginBody pluginBody, string configKey, string childKey)
        {
            return ((JObject) pluginBody.config[configKey]).SafeCastJObjectProperty<string[]>(childKey) ?? new string[]{};
        }

        public override PluginBody DoCreatePluginBody(RequestTransformerAdvancedConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "http_method", target.HttpMethod },

                { "remove.headers", string.Join(',', target.Remove.Headers) },
                { "remove.querystring", string.Join(',', target.Remove.QueryString) },
                { "remove.body", string.Join(',', target.Remove.Body) },

                { "replace.headers", string.Join(',', target.Replace.Headers) },
                { "replace.querystring", string.Join(',', target.Replace.QueryString) },
                { "replace.body", string.Join(',', target.Replace.Body) },
                { "replace.uri", target.Replace.Uri },

                { "rename.headers", string.Join(',', target.Rename.Headers) },
                { "rename.querystring", string.Join(',', target.Rename.QueryString) },
                { "rename.body", string.Join(',', target.Rename.Body) },

                { "add.headers", string.Join(',', target.Add.Headers) },
                { "add.querystring", string.Join(',', target.Add.QueryString) },
                { "add.body", string.Join(',', target.Add.Body) },

                { "append.headers", string.Join(',', target.Append.Headers) },
                { "append.querystring", string.Join(',', target.Append.QueryString) },
                { "append.body", string.Join(',', target.Append.Body) },
            });
        }
    }
}
