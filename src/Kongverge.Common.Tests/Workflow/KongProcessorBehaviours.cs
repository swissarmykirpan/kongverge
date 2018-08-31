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
using Newtonsoft.Json;

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
        protected IReadOnlyList<TestPlugins.FakePluginConfig> Plugins;

        public override void Setup()
        {
            var fixture = new Fixture();
            GlobalConfigs = TestGlobalConfigs.Create(fixture);
            Services = TestServices.Create(fixture);
            Routes = TestRoutes.Create(fixture);
            Plugins = TestPlugins.Create();

            Mock.Get(The<IKongPluginCollection>())
                .Setup(x => x.CreatePluginBody(It.IsAny<TestPlugins.FakePluginConfig>()))
                .Returns<TestPlugins.FakePluginConfig>(x => x.CreatePluginBody());

            Mock.Get(The<IKongAdminWriter>())
                .Setup(x => x.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()))
                .Returns<KongService, KongRoute>((service, route) =>
                {
                    route.AsExisting();
                    return Task.CompletedTask;
                });

            Mock.Get(The<IKongAdminWriter>())
                .Setup(x => x.UpsertPlugin(It.IsAny<PluginBody>()))
                .Returns<PluginBody>(x =>
                {
                    if (x.id == null)
                    {
                        x.id = Guid.NewGuid().ToString();
                    }
                    return Task.CompletedTask;
                });

            Mock.Get(The<IKongAdminWriter>())
                .Setup(x => x.AddService(It.IsAny<KongService>()))
                .Returns<KongService>(x =>
                {
                    x.AsExisting();
                    return Task.CompletedTask;
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
            TargetServices = new[]
            {
                Services[0].AsTarget(),
                Services[1].AsTarget(),
                Services[2].AsTarget()
            };

            TargetServices[0].Routes = new[]
            {
                Routes[0].AsTarget(),
                Routes[1].AsTarget()
            };
            TargetServices[0].Routes[0].Plugins = new[]
            {
                Plugins[0].AsTarget(),
                Plugins[1].AsTarget()
            };
            TargetServices[0].Routes[1].Plugins = new[]
            {
                Plugins[2].AsTarget()
            };
            TargetServices[1].Routes = new[]
            {
                Routes[2].AsTarget()
            };
            TargetServices[2].Routes = new[]
            {
                Routes[3].AsTarget(),
                Routes[4].AsTarget()
            };

            TargetServices[0].Plugins = new[]
            {
                Plugins[3].AsTarget(),
                Plugins[4].AsTarget()
            };
            TargetServices[1].Plugins = new[]
            {
                Plugins[5].AsTarget()
            };
            TargetServices[2].Plugins = new[]
            {
                Plugins[3].AsTarget(),
                Plugins[4].AsTarget()
            };

            TargetGlobalConfig = GlobalConfigs[0];

            TargetGlobalConfig.Plugins = new[]
            {
                Plugins[6].AsTarget(),
                Plugins[7].AsTarget()
            };
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

                public void AndNoPluginsAreDeleted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(It.IsAny<string>()), Times.Never);
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

        public static KongService AsExisting(this KongService kongService)
        {
            kongService.Id = Guid.NewGuid().ToString();
            return kongService;
        }

        public static KongService AsTarget(this KongService kongService, bool modified = false)
        {
            var serlialized = JsonConvert.SerializeObject(kongService);
            var target = JsonConvert.DeserializeObject<KongService>(serlialized);
            if (modified)
            {
                target.Host = Guid.NewGuid().ToString();
            }
            return target;
        }

        private static KongService CreateService(Fixture fixture) =>
            fixture
                .Build<KongService>()
                .Without(x => x.Id)
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

        public static KongRoute AsExisting(this KongRoute kongRoute)
        {
            kongRoute.Id = Guid.NewGuid().ToString();
            return kongRoute;
        }

        public static KongRoute AsTarget(this KongRoute kongRoute, bool modified = false)
        {
            var serlialized = JsonConvert.SerializeObject(kongRoute);
            var target = JsonConvert.DeserializeObject<KongRoute>(serlialized);
            if (modified)
            {
                target.Hosts = new [] { Guid.NewGuid().ToString() };
            }
            return target;
        }

        private static KongRoute CreateRoute(Fixture fixture) =>
            fixture
                .Build<KongRoute>()
                .Without(x => x.Id)
                .Without(x => x.Plugins)
                .Create();
    }

    public static class TestPlugins
    {
        class FakePluginConfig1 : FakePluginConfig { }
        class FakePluginConfig2 : FakePluginConfig { }
        class FakePluginConfig3 : FakePluginConfig { }
        class FakePluginConfig4 : FakePluginConfig { }
        class FakePluginConfig5 : FakePluginConfig { }
        class FakePluginConfig6 : FakePluginConfig { }
        class FakePluginConfig7 : FakePluginConfig { }
        class FakePluginConfig8 : FakePluginConfig { }
        class FakePluginConfig9 : FakePluginConfig { }

        public static IReadOnlyList<FakePluginConfig> Create() =>
            new FakePluginConfig[]
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
            config.id = Guid.NewGuid().ToString();
            return config;
        }

        public static FakePluginConfig AsTarget(this FakePluginConfig config, bool modified = false)
        {
            var target = Activator.CreateInstance(config.GetType());
            var targetAsFakePluginConfig = ((FakePluginConfig)target);
            targetAsFakePluginConfig.DummyProperty = modified
                ? Guid.NewGuid().ToString()
                : config.DummyProperty;
            return targetAsFakePluginConfig;
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
