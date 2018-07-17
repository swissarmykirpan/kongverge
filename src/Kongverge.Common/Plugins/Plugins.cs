using Kongverge.Extension;
using System.Collections.Generic;

namespace Kongverge.Common.Plugins
{
    public static class Plugins
    {
        public static PluginBody Http
        {
            get
            {
                return new PluginBody("http-log", new Dictionary<string, object>{
                    {"test", 1 }
                });
            }
        }
    }
}
