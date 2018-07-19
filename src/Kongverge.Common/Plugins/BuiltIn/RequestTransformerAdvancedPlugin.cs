using System.Collections.Generic;
using System.Linq;
using Kongverge.Common.Helpers;
using Kongverge.Extension;
using Newtonsoft.Json.Linq;

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
                    Headers = new HashSet<string>(((JObject) pluginBody.config["remove"]).SafeCastJObjectProperty<string[]>("headers") ?? new string[]{}),
                    QueryString = new HashSet<string>(((JObject)pluginBody.config["remove"]).SafeCastJObjectProperty<string[]>("querystring") ?? new string[] { }),
                    Body = new HashSet<string>(((JObject)pluginBody.config["remove"]).SafeCastJObjectProperty<string[]>("body") ?? new string[] { })
                },

                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Headers = new HashSet<string>(((JObject)pluginBody.config["replace"]).SafeCastJObjectProperty<string[]>("headers") ?? new string[] { }),
                    QueryString = new HashSet<string>(((JObject)pluginBody.config["replace"]).SafeCastJObjectProperty<string[]>("querystring") ?? new string[] { }),
                    Body = new HashSet<string>(((JObject)pluginBody.config["replace"]).SafeCastJObjectProperty<string[]>("body") ?? new string[] { }),
                    Uri = ((JObject)pluginBody.config["replace"]).SafeCastJObjectProperty<string>("uri")
                },

                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(((JObject)pluginBody.config["rename"]).SafeCastJObjectProperty<string[]>("headers") ?? new string[] { }),
                    QueryString = new HashSet<string>(((JObject)pluginBody.config["rename"]).SafeCastJObjectProperty<string[]>("querystring") ?? new string[] { }),
                    Body = new HashSet<string>(((JObject)pluginBody.config["rename"]).SafeCastJObjectProperty<string[]>("body") ?? new string[] { })
                },

                Add = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(((JObject)pluginBody.config["add"]).SafeCastJObjectProperty<string[]>("headers") ?? new string[] { }),
                    QueryString = new HashSet<string>(((JObject)pluginBody.config["add"]).SafeCastJObjectProperty<string[]>("querystring") ?? new string[] { }),
                    Body = new HashSet<string>(((JObject)pluginBody.config["add"]).SafeCastJObjectProperty<string[]>("body") ?? new string[] { })
                },

                Append = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string>(((JObject)pluginBody.config["append"]).SafeCastJObjectProperty<string[]>("headers") ?? new string[] { }),
                    QueryString = new HashSet<string>(((JObject)pluginBody.config["append"]).SafeCastJObjectProperty<string[]>("querystring") ?? new string[] { }),
                    Body = new HashSet<string>(((JObject)pluginBody.config["append"]).SafeCastJObjectProperty<string[]>("body") ?? new string[] { })
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
