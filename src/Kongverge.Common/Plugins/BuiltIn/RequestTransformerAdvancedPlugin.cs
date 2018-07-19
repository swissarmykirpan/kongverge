using System.Collections.Generic;
using System.Linq;
using Kongverge.Extension;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTransformerAdvancedPlugin : ExtensionBase<RequestTransformerAdvancedConfig>
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
                    // TODO: Need to fix this - seems to be a JObject, so need to cast accordingly
                    Headers = new HashSet<string>(((string) pluginBody.config["remove.headers"]).Split(',')),
                    QueryString = new HashSet<string>(((string) pluginBody.config["remove.querystring"]).Split(',')),
                    Body = new HashSet<string>(((string) pluginBody.config["remove.body"]).Split(','))
                },

                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Headers = new HashSet<string>(((string)pluginBody.config["replace.headers"]).Split(',')),
                    QueryString = new HashSet<string>(((string)pluginBody.config["replace.querystring"]).Split(',')),
                    Body = new HashSet<string>(((string)pluginBody.config["replace.body"]).Split(',')),
                    Uri = (string)pluginBody.config["replace.uri"]
                },

                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(((string)pluginBody.config["rename.headers"]).Split(',')),
                    QueryString = new HashSet<string>(((string)pluginBody.config["rename.querystring"]).Split(',')),
                    Body = new HashSet<string>(((string)pluginBody.config["rename.body"]).Split(','))
                },

                Add = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(((string)pluginBody.config["add.headers"]).Split(',')),
                    QueryString = new HashSet<string>(((string)pluginBody.config["add.querystring"]).Split(',')),
                    Body = new HashSet<string>(((string)pluginBody.config["add.body"]).Split(','))
                },

                Append = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(((string)pluginBody.config["append.headers"]).Split(',')),
                    QueryString = new HashSet<string>(((string)pluginBody.config["append.querystring"]).Split(',')),
                    Body = new HashSet<string>(((string)pluginBody.config["append.body"]).Split(','))
                },
            };
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
