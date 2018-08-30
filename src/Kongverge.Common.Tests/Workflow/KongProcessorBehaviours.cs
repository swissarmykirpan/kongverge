using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Kongverge.Common.Workflow;
using Kongverge.KongPlugin;
using Moq;

namespace Kongverge.Common.Tests.Workflow
{
    public abstract class WhenProcessingServicesAndGlobalConfigChanges : ScenarioFor<KongProcessor>
    {
        protected IReadOnlyList<KongService> ExistingServices;
        protected GlobalConfig ExistingGlobalConfig;
        protected IReadOnlyList<KongService> TargetServices;
        protected GlobalConfig TargetGlobalConfig;

        protected IReadOnlyList<GlobalConfig> GlobalConfigs;
        protected IReadOnlyList<KongService> Services;
        protected IReadOnlyList<KongRoute> Routes;
        protected IReadOnlyList<IKongPluginConfig> Plugins;

        public override void Setup()
        {
            var fixture = new Fixture();
            GlobalConfigs = TestGlobalConfigs.Create(fixture);
            Services = TestServices.Create(fixture);
            Routes = TestRoutes.Create(fixture);
            Plugins = TestPlugins.Create();
            Mock.Get(The<IKongPluginCollection>())
                .Setup(x => x.CreatePluginBody(It.IsAny<IKongPluginConfig>()))
                .Returns<IKongPluginConfig>(x =>
                {
                    var pluginBody = fixture
                        .Build<PluginBody>()
                        .Without(p => p.route_id)
                        .Without(p => p.service_id)
                        .Without(p => p.consumer_id)
                        .Create();
                    pluginBody.LinkToPluginConfig(x);
                    return pluginBody;
                });
        }

        protected void SetupNoExistingServicesOrGlobalConfig()
        {
            ExistingServices = Array.Empty<KongService>();
            ExistingGlobalConfig = null;
        }

        protected void SetupAnAssortmentOfExistingServicesAndGlobalConfig()
        {
            ExistingServices = Array.Empty<KongService>();
            ExistingGlobalConfig = null;
        }

        protected void SetupNoTargetServicesOrGlobalConfig()
        {
            TargetServices = Array.Empty<KongService>();
            TargetGlobalConfig = null;
        }

        protected void SetupAnAssortmentOfTargetServicesAndGlobalConfig()
        {
            Routes[0].Plugins = new[]
            {
                Plugins[0],
                Plugins[1]
            };

            Routes[1].Plugins = new[]
            {
                Plugins[2]
            };

            Services[0].Routes = new[]
            {
                Routes[0],
                Routes[1]
            };
            Services[1].Routes = new[]
            {
                Routes[2]
            };
            Services[2].Routes = new[]
            {
                Routes[3],
                Routes[4]
            };

            Services[0].Plugins = new[]
            {
                Plugins[3],
                Plugins[4]
            };
            Services[1].Plugins = new[]
            {
                Plugins[5]
            };
            Services[2].Plugins = new[]
            {
                Plugins[3],
                Plugins[4]
            };

            GlobalConfigs[0].Plugins = new[]
            {
                Plugins[6],
                Plugins[7]
            };

            TargetServices = new[]
            {
                Services[0],
                Services[1],
                Services[2]
            };

            TargetGlobalConfig = GlobalConfigs[0];
        }

        protected async Task WhenProcessing() =>
            await SUT.Process(ExistingServices, TargetServices, ExistingGlobalConfig, TargetGlobalConfig);

        public abstract class WithNoExistingServicesOrGlobalConfig : WhenProcessingServicesAndGlobalConfigChanges
        {
            protected void GivenNoExistingServicesOrGlobalConfig()
            {
                SetupNoExistingServicesOrGlobalConfig();
            }

            public class AndNoTargetServicesOrGlobalConfig : WithNoExistingServicesOrGlobalConfig
            {
                public void AndGivenNoTargetServicesOrGlobalConfig() =>
                    SetupNoTargetServicesOrGlobalConfig();

                public void ThenNoServicesAreAdded() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddService(It.IsAny<KongService>()), Times.Never);

                public void AndNoServicesAreUpdated() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpdateService(It.IsAny<KongService>()), Times.Never);

                public void AndNoServicesAreDeleted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteService(It.IsAny<string>()), Times.Never);

                public void AndNoRoutesAreAdded() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Never);

                public void AndNoRoutesAreDeleted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteRoute(It.IsAny<string>()), Times.Never);

                public void AndNoPluginsAreUpserted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.IsAny<PluginBody>()), Times.Never);

                public void AndNoPluginsAreDeleted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(It.IsAny<string>()), Times.Never);
            }

            public class AndAnAssortmentOfTargetServicesAndGlobalConfig : WithNoExistingServicesOrGlobalConfig
            {
                public void AndGivenAnAssortmentOfTargetServicesAndGlobalConfig() =>
                    SetupAnAssortmentOfTargetServicesAndGlobalConfig();

                public void ThenTheTargetServicesAreAdded()
                {
                    foreach (var targetService in TargetServices)
                    {
                        Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddService(targetService), Times.Once);
                    }
                }

                public void AndNoServicesAreUpdated() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpdateService(It.IsAny<KongService>()), Times.Never);

                public void AndNoServicesAreDeleted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteService(It.IsAny<string>()), Times.Never);

                public void AndTheTargetRoutesAreAdded()
                {
                    foreach (var targetService in TargetServices)
                    {
                        foreach (var targetRoute in targetService.Routes)
                        {
                            Mock.Get(The<IKongAdminWriter>())
                                .Verify(x => x.AddRoute(targetService, targetRoute), Times.Once);
                        }
                    }
                }

                public void AndNoRoutesAreDeleted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteRoute(It.IsAny<string>()), Times.Never);

                public void AndTheTargetPluginsAreUpserted()
                {
                    foreach (var targetService in TargetServices)
                    {
                        foreach (var targetPlugin in targetService.Plugins)
                        {
                            Mock.Get(The<IKongAdminWriter>())
                                .Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p => p.CorrespondsToPluginConfig(targetPlugin) && p.CorrespondsToKongService(targetService))), Times.Once);
                        }
                        foreach (var targetRoute in targetService.Routes)
                        {
                            foreach (var targetPlugin in targetRoute.Plugins)
                            {
                                Mock.Get(The<IKongAdminWriter>())
                                    .Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p => p.CorrespondsToPluginConfig(targetPlugin) && p.CorrespondsToKongRoute(targetRoute))), Times.Once);
                            }
                        }
                    }
                    foreach (var targetPlugin in TargetGlobalConfig.Plugins)
                    {
                        Mock.Get(The<IKongAdminWriter>())
                            .Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p => p.CorrespondsToPluginConfig(targetPlugin) && p.IsGlobalPlugin())), Times.Once);
                    }
                }

                public void AndNoPluginsAreDeleted()
                    => Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(It.IsAny<string>()), Times.Never);
            }
        }
    }

    public static class TestGlobalConfigs
    {
        public static IReadOnlyList<GlobalConfig> Create(Fixture fixture) =>
            new[]
            {
                CreateGlobalConfig(fixture),
                CreateGlobalConfig(fixture),
                CreateGlobalConfig(fixture),
                CreateGlobalConfig(fixture),
                CreateGlobalConfig(fixture)
            };

        private static GlobalConfig CreateGlobalConfig(Fixture fixture) =>
            fixture
                .Build<GlobalConfig>()
                .Without(x => x.Plugins)
                .Create();
    }

    public static class TestServices
    {
        public static IReadOnlyList<KongService> Create(Fixture fixture) =>
            new[]
            {
                CreateService(fixture),
                CreateService(fixture),
                CreateService(fixture),
                CreateService(fixture),
                CreateService(fixture)
            };

        private static KongService CreateService(Fixture fixture)
            => fixture
                .Build<KongService>()
                .With(x => x.ValidateHost, false)
                .Without(x => x.Routes)
                .Without(x => x.Plugins)
                .Create();
    }

    public static class TestRoutes
    {
        public static IReadOnlyList<KongRoute> Create(Fixture fixture) =>
            new[]
            {
                CreateRoute(fixture),
                CreateRoute(fixture),
                CreateRoute(fixture),
                CreateRoute(fixture),
                CreateRoute(fixture)
            };

        private static KongRoute CreateRoute(Fixture fixture)
            => fixture
                .Build<KongRoute>()
                .Without(x => x.Plugins)
                .Create();
    }

    public static class TestPlugins
    {
        public static IReadOnlyList<IKongPluginConfig> Create()
            => new IKongPluginConfig[]
            {
                new FakePluginConfig1(),
                new FakePluginConfig2(),
                new FakePluginConfig3(),
                new FakePluginConfig4(),
                new FakePluginConfig5(),
                new FakePluginConfig6(),
                new FakePluginConfig7(),
                new FakePluginConfig8(),
                new FakePluginConfig9()
            };

        public static void LinkToPluginConfig(this PluginBody pluginBody, IKongPluginConfig pluginConfig)
            => pluginBody.name = pluginConfig.GetType().Name;

        public static bool CorrespondsToPluginConfig(this PluginBody pluginBody, IKongPluginConfig pluginConfig)
            => pluginBody.name == pluginConfig.GetType().Name;

        public static bool CorrespondsToKongService(this PluginBody pluginBody, KongService kongService)
            => pluginBody.service_id == kongService.Id &&
               pluginBody.consumer_id == null &&
               pluginBody.route_id == null;

        public static bool CorrespondsToKongRoute(this PluginBody pluginBody, KongRoute kongRoute)
            => pluginBody.route_id == kongRoute.Id &&
               pluginBody.consumer_id == null &&
               pluginBody.service_id == null;

        public static bool IsGlobalPlugin(this PluginBody pluginBody)
            => pluginBody.route_id == null &&
               pluginBody.service_id == null;

        class FakePluginConfig1 : FakePluginConfigBase { }
        class FakePluginConfig2 : FakePluginConfigBase { }
        class FakePluginConfig3 : FakePluginConfigBase { }
        class FakePluginConfig4 : FakePluginConfigBase { }
        class FakePluginConfig5 : FakePluginConfigBase { }
        class FakePluginConfig6 : FakePluginConfigBase { }
        class FakePluginConfig7 : FakePluginConfigBase { }
        class FakePluginConfig8 : FakePluginConfigBase { }
        class FakePluginConfig9 : FakePluginConfigBase { }

        public abstract class FakePluginConfigBase : IKongPluginConfig
        {
            public string DummyProperty { get; set; } = Guid.NewGuid().ToString();

            public string id { get; set; }

            public bool IsExactMatch(IKongPluginConfig other)
            {
                if (other is FakePluginConfigBase otherConfig)
                {
                    return DummyProperty == otherConfig.DummyProperty;
                }

                return false;
            }

            public void MakeExisting()
                => id = Guid.NewGuid().ToString();

            public void MakeChanged()
                => DummyProperty = Guid.NewGuid().ToString();
        }
    }
}
