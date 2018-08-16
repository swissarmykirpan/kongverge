using System.Collections.Generic;
using Kongverge.KongPlugin;
using Newtonsoft.Json.Linq;

namespace Kongverge.Common.Plugins.BuiltIn.RequestTransform
{
    public abstract class BaseRequestTransformerPlugin<T> : KongPluginBase<T> where T : IRequestTransformerConfig, new()
    {
        protected BaseRequestTransformerPlugin(string section) : base(section)
        {
        }

        protected override T DoCreateConfigObject(PluginBody pluginBody)
        {
            var httpMethod = pluginBody.config.ReadString("http_method");

            var addData = pluginBody.config.SubProperties("add");
            var appendData = pluginBody.config.SubProperties("append");
            var removeData = pluginBody.config.SubProperties("remove");
            var renameData = pluginBody.config.SubProperties("rename");
            var replaceData = pluginBody.config.SubProperties("replace");

            var replaceConfig = ReadSection<RequestTransformerAdvancedTransformReplace>(replaceData);
            replaceConfig.Uri = replaceData.ReadString("uri");

            return new T
            {
                HttpMethod = httpMethod,
                Remove = ReadRemoveSection(removeData),
                Replace = replaceConfig,
                Rename = ReadSection<RequestTransformerAdvancedTransformBase>(renameData),
                Add = ReadSection<RequestTransformerAdvancedTransformBase>(addData),
                Append = ReadSection<RequestTransformerAdvancedTransformBase>(appendData)
            };
        }

        private static C ReadSection<C>(IDictionary<string, object> section)
            where C: RequestTransformerAdvancedTransformBase, new()
        {
            return new C
            {
                Headers = section.ReadStringSet("headers"),
                QueryString = section.ReadStringSet("querystring"),
                Body = section.ReadStringSet("body")
            };
        }

        private static RequestTransformerAdvancedTransformRemove ReadRemoveSection(IDictionary<string, object> section)
        {
            return new RequestTransformerAdvancedTransformRemove
            {
                Headers = section.ReadStringSet("headers"),
                QueryString = section.ReadStringSet("querystring"),
                Body = section.ReadStringSet("body")
            };
        }

        protected override PluginBody DoCreatePluginBody(T target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "http_method", target.HttpMethod },

                { "add", WriteSection(target.Add) },
                { "append", WriteSection(target.Append) },
                { "remove", WriteRemoveSection(target.Remove) },
                { "rename", WriteSection(target.Rename) },
                { "replace", WriteReplaceSection(target.Replace) }
            });
        }

        private static JObject WriteSection(RequestTransformerAdvancedTransformBase section)
        {
            return new JObject
            {
                {"headers", section.Headers.ToCommaSeperatedString()},
                {"querystring", section.QueryString.ToCommaSeperatedString()},
                {"body",  section.Body.ToCommaSeperatedString()}
            };
        }

        private static JObject WriteRemoveSection(RequestTransformerAdvancedTransformRemove section)
        {
            return new JObject
            {
                {"headers", section.Headers.ToCommaSeperatedString()},
                {"querystring", section.QueryString.ToCommaSeperatedString()},
                {"body",  section.Body.ToCommaSeperatedString()}
            };
        }

        private static JObject WriteReplaceSection(RequestTransformerAdvancedTransformReplace section)
        {
            return new JObject
            {
                { "uri", section.Uri },
                {"headers", section.Headers.ToCommaSeperatedString()},
                {"querystring", section.QueryString.ToCommaSeperatedString()},
                {"body",  section.Body.ToCommaSeperatedString()}
            };
        }
    }
}
