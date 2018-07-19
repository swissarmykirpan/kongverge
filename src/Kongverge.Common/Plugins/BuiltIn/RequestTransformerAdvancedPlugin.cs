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
                // TODO: How do we access consumerId in this case? We only get the config dictionary?
                //ConsumerId = (string)pluginBody.config["consumer_id"],
                HttpMethod = (string) pluginBody.config["http_method"],

                Remove = new RequestTransformerAdvancedTransformRemove
                {
                    Headers = new HashSet<string>(((string) pluginBody.config["remove.headers"]).Split(',')),
                    QueryString = new HashSet<string>(((string) pluginBody.config["remove.querystring"]).Split(',')),
                    Body = new HashSet<string>(((string) pluginBody.config["remove.body"]).Split(','))
                },

                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Headers = new Dictionary<string, string>(((string) pluginBody.config["replace.headers"]).Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        })),
                    QueryString = new Dictionary<string, string>(((string) pluginBody.config["replace.querystring"])
                        .Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        })),
                    Body = new Dictionary<string, string>(((string) pluginBody.config["replace.body"]).Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        })),
                    Uri = (string) pluginBody.config["replace.uri"]
                },

                Rename = new RequestTransformerAdvancedTransformKpBase
                {
                    Headers = new Dictionary<string, string>(((string) pluginBody.config["rename.headers"]).Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        })),
                    QueryString = new Dictionary<string, string>(((string) pluginBody.config["rename.querystring"])
                        .Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        })),
                    Body = new Dictionary<string, string>(((string) pluginBody.config["rename.body"]).Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        }))
                },

                Add = new RequestTransformerAdvancedTransformKpBase
                {
                    Headers = new Dictionary<string, string>(((string) pluginBody.config["add.headers"]).Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        })),
                    QueryString = new Dictionary<string, string>(((string) pluginBody.config["add.querystring"])
                        .Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        })),
                    Body = new Dictionary<string, string>(((string) pluginBody.config["add.body"]).Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        }))
                },

                Append = new RequestTransformerAdvancedTransformKpBase
                {
                    Headers = new Dictionary<string, string>(((string) pluginBody.config["append.headers"]).Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        })),
                    QueryString = new Dictionary<string, string>(((string) pluginBody.config["append.querystring"])
                        .Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        })),
                    Body = new Dictionary<string, string>(((string) pluginBody.config["append.body"]).Split(',')
                        .Select(x =>
                        {
                            var kp = x.Split(':');
                            return new KeyValuePair<string, string>(kp[0], kp[1]);
                        }))
                },
            };
        }

        public override PluginBody DoCreatePluginBody(RequestTransformerAdvancedConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                //{ "consumer_id", target.ConsumerId },
                { "http_method", target.HttpMethod },

                { "remove.headers", string.Join(',', target.Remove.Headers) },
                { "remove.querystring", string.Join(',', target.Remove.QueryString) },
                { "remove.body", string.Join(',', target.Remove.Body) },

                { "replace.headers", string.Join(',', target.Replace.Headers.Select(kp => $"{kp.Key}:{kp.Value}")) },
                { "replace.querystring", string.Join(',', target.Replace.QueryString.Select(kp => $"{kp.Key}:{kp.Value}")) },
                { "replace.body", string.Join(',', target.Replace.Body.Select(kp => $"{kp.Key}:{kp.Value}")) },
                { "replace.uri", target.Replace.Uri },

                { "rename.headers", string.Join(',', target.Rename.Headers.Select(kp => $"{kp.Key}:{kp.Value}")) },
                { "rename.querystring", string.Join(',', target.Rename.QueryString.Select(kp => $"{kp.Key}:{kp.Value}")) },
                { "rename.body", string.Join(',', target.Rename.Body.Select(kp => $"{kp.Key}:{kp.Value}")) },

                { "add.headers", string.Join(',', target.Add.Headers.Select(kp => $"{kp.Key}:{kp.Value}")) },
                { "add.querystring", string.Join(',', target.Add.QueryString.Select(kp => $"{kp.Key}:{kp.Value}")) },
                { "add.body", string.Join(',', target.Add.Body.Select(kp => $"{kp.Key}:{kp.Value}")) },

                { "append.headers", string.Join(',', target.Append.Headers.Select(kp => $"{kp.Key}:{kp.Value}")) },
                { "append.querystring", string.Join(',', target.Append.QueryString.Select(kp => $"{kp.Key}:{kp.Value}")) },
                { "append.body", string.Join(',', target.Append.Body.Select(kp => $"{kp.Key}:{kp.Value}")) },
            });
        }
    }
}
