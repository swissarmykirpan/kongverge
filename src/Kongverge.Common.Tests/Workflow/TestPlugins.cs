using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AutoFixture;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;
using Kongverge.TestHelpers;

namespace Kongverge.Common.Tests.Workflow
{
    public static class TestPlugins
    {
        public static IReadOnlyList<FakePluginConfig> CreatePlugins(this Fixture fixture, int count) =>
            Enumerable.Range(0, count).Select(CreatePlugin).ToArray();

        private static FakePluginConfig CreatePlugin(int number)
        {
            var moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run)
                .DefineDynamicModule("MainModule");
            var parentType = typeof(FakePluginConfig);
            var typeBuilder = moduleBuilder.DefineType(parentType.Name + number,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                parentType);

            return (FakePluginConfig)Activator.CreateInstance(typeBuilder.CreateType());
        }

        public static bool CorrespondsToPluginConfig(this PluginBody pluginBody, IKongPluginConfig pluginConfig) =>
            pluginBody.name == pluginConfig.GetType().Name;

        public static bool CorrespondsToKongService(this PluginBody pluginBody, KongService kongService) =>
            pluginBody.service_id == kongService.Id &&
            pluginBody.consumer_id == null &&
            pluginBody.route_id == null;

        public static bool CorrespondsToKongRoute(this PluginBody pluginBody, KongRoute kongRoute) =>
            pluginBody.route_id == kongRoute.Id &&
            pluginBody.consumer_id == null &&
            pluginBody.service_id == null;

        public static bool IsGlobalPlugin(this PluginBody pluginBody) =>
            pluginBody.route_id == null &&
            pluginBody.service_id == null;

        public static FakePluginConfig AsExisting(this FakePluginConfig config)
        {
            var existing = config.Clone();
            existing.id = Guid.NewGuid().ToString();
            return existing;
        }

        public static PluginBody WithId(this PluginBody pluginBody)
        {
            if (pluginBody.id == null)
            {
                pluginBody.id = Guid.NewGuid().ToString();
            }
            return pluginBody;
        }

        public static FakePluginConfig AsTarget(this FakePluginConfig config, bool modified = false)
        {
            var target = config.Clone();
            if (modified)
            {
                target.DummyProperty = Guid.NewGuid().ToString();
            }
            return target;
        }

        public class FakePluginConfig : IKongPluginConfig
        {
            public string DummyProperty { get; set; } = Guid.NewGuid().ToString();

            public string id { get; set; }

            public bool IsExactMatch(IKongPluginConfig other)
            {
                if (other is FakePluginConfig otherConfig)
                {
                    return DummyProperty == otherConfig.DummyProperty;
                }

                return false;
            }

            public PluginBody CreatePluginBody()
            {
                return new PluginBody(GetType().Name, new Dictionary<string, object>
                {
                    { nameof(DummyProperty), DummyProperty }
                });
            }
        }
    }
}
