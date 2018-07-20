using System;

namespace Kongverge.Extension
{
    public interface IKongPlugin
    {
        string SectionName { get; }

        string PluginName { get; }

        Type KongObjectType { get; }

        PluginBody CreatePluginBody(IKongPluginConfig target);

        IKongPluginConfig CreateConfigObject(PluginBody pluginBody);
    }
}