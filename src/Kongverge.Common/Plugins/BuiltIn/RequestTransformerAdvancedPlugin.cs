using System.Collections.Generic;
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

        protected override RequestTransformerAdvancedConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new RequestTransformerAdvancedConfig
            {
                HttpMethod = (string) pluginBody.config["http_method"],

                Remove = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(pluginBody.ReadConfigStringArray("remove.headers")),
                    QueryString = new HashSet<string>(pluginBody.ReadConfigStringArray("remove.querystring")),
                    Body = new HashSet<string>(pluginBody.ReadConfigStringArray("remove.body"))
                },

                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Headers = new HashSet<string>(pluginBody.ReadConfigStringArray("replace.headers")),
                    QueryString = new HashSet<string>(pluginBody.ReadConfigStringArray("replace.querystring")),
                    Body = new HashSet<string>(pluginBody.ReadConfigStringArray("replace.body")),
                    Uri = pluginBody.ReadConfigString("replace.uri")
                },

                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(pluginBody.ReadConfigStringArray("rename.headers")),
                    QueryString = new HashSet<string>(pluginBody.ReadConfigStringArray("rename.querystring")),
                    Body = new HashSet<string>(pluginBody.ReadConfigStringArray("rename.body"))
                },

                Add = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(pluginBody.ReadConfigStringArray("add.headers")),
                    QueryString = new HashSet<string>(pluginBody.ReadConfigStringArray("add.querystring")),
                    Body = new HashSet<string>(pluginBody.ReadConfigStringArray("add.body"))
                },

                Append = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(pluginBody.ReadConfigStringArray("append.headers")),
                    QueryString = new HashSet<string>(pluginBody.ReadConfigStringArray("append.querystring")),
                    Body = new HashSet<string>(pluginBody.ReadConfigStringArray("append.body"))
                },
            };
        }

        protected override PluginBody DoCreatePluginBody(RequestTransformerAdvancedConfig target)
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
