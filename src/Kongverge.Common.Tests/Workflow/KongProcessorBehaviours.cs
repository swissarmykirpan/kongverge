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
        protected IReadOnlyList<TestPlugins.FakePluginConfig> Plugins;

        public override void Setup()
        {
            var fixture = new Fixture();
            GlobalConfigs = fixture.CreateGlobalConfigs(2);
            Services = fixture.CreateServices(4);
            Routes = fixture.CreateRoutes(6);
            Plugins = fixture.CreatePlugins(10);

            Mock.Get(The<IKongPluginCollection>())
                .Setup(x => x.CreatePluginBody(It.IsAny<TestPlugins.FakePluginConfig>()))
                .Returns<TestPlugins.FakePluginConfig>(x => x.CreatePluginBody());

            Mock.Get(The<IKongAdminWriter>())
                .Setup(x => x.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()))
                .Returns<KongService, KongRoute>((service, route) =>
                {
                    route.WithId();
                    return Task.CompletedTask;
                });

            Mock.Get(The<IKongAdminWriter>())
                .Setup(x => x.UpsertPlugin(It.IsAny<PluginBody>()))
                .Returns<PluginBody>(x =>
                {
                    x.WithId();
                    return Task.CompletedTask;
                });

            Mock.Get(The<IKongAdminWriter>())
                .Setup(x => x.AddService(It.IsAny<KongService>()))
                .Returns<KongService>(x =>
                {
                    x.WithId();
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
            ExistingServices = new[]
            {
                Services[0].AsExisting(),
                Services[1].AsExisting(),
                Services[2].AsExisting()
            };

            ExistingServices[0].Routes = new[]
            {
                Routes[0].AsExisting(),
                Routes[1].AsExisting(),
                Routes[2].AsExisting()
            };
            ExistingServices[0].Routes[0].Plugins = new[]
            {
                Plugins[0].AsExisting(),
                Plugins[1].AsExisting(),
                Plugins[2].AsExisting()
            };
            ExistingServices[0].Routes[1].Plugins = new[]
            {
                Plugins[3].AsExisting(),
                Plugins[4].AsExisting(),
                Plugins[5].AsExisting()
            };

            ExistingServices[1].Routes = new[]
            {
                Routes[3].AsExisting()
            };
            ExistingServices[1].Plugins = new[]
            {
                Plugins[3].AsExisting(),
                Plugins[4].AsExisting(),
                Plugins[5].AsExisting()
            };
            
            ExistingServices[2].Routes = new[]
            {
                Routes[4].AsExisting(),
                Routes[5].AsExisting()
            };

            ExistingGlobalConfig = GlobalConfigs[0];
            ExistingGlobalConfig.Plugins = new[]
            {
                Plugins[6].AsExisting(),
                Plugins[7].AsExisting(),
                Plugins[8].AsExisting()
            };
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
                Services[0].AsTarget(true), // Changed
                Services[1].AsTarget(), // Same
                // Services[2] Removed
                Services[3].AsTarget() // Added
            };

            TargetServices[0].Routes = new[]
            {
                Routes[0].AsTarget(true), // Changed
                Routes[1].AsTarget(), // Same
                // Routes[2] Removed
                Routes[3].AsTarget() // Added
            };
            TargetServices[0].Routes[0].Plugins = new[]
            {
                Plugins[0].AsTarget(true), // Changed
                Plugins[1].AsTarget(), // Same
                // Plugins[2] Removed
                Plugins[3].AsTarget() // Added
            };
            TargetServices[0].Routes[1].Plugins = new[]
            {
                Plugins[3].AsTarget(true), // Changed
                Plugins[4].AsTarget(), // Same
                // Plugins[5] Removed
                Plugins[6].AsTarget() // Added
            };

            TargetServices[1].Routes = new[]
            {
                // Routes[3] Removed
                Routes[4].AsTarget() // Added
            };
            TargetServices[1].Plugins = new[]
            {
                Plugins[3].AsTarget(true), // Changed
                Plugins[4].AsTarget(), // Same
                // Plugins[5] Removed
                Plugins[6].AsTarget() // Added
            };

            TargetServices[2].Routes = new[]
            {
                Routes[5].AsTarget() // Added
            };
            TargetServices[2].Routes[0].Plugins = new[]
            {
                Plugins[0].AsTarget(), // Added
                Plugins[1].AsTarget() // Added
            };
            TargetServices[0].Plugins = new[]
            {
                Plugins[2].AsTarget(), // Added
                Plugins[3].AsTarget() // Added
            };

            TargetGlobalConfig = GlobalConfigs[1];
            TargetGlobalConfig.Plugins = new[]
            {
                Plugins[6].AsTarget(true), // Changed
                Plugins[7].AsTarget(), // Same
                // Plugins[8] Removed
                Plugins[9].AsTarget() // Added
            };
        }

        protected async Task WhenProcessing() =>
            await SUT.Process(ExistingServices, TargetServices, ExistingGlobalConfig, TargetGlobalConfig);

        public abstract class WithNoExistingServicesOrGlobalConfig : WhenProcessingServicesAndGlobalConfigChanges
        {
            protected void GivenNoExistingServicesOrGlobalConfig() =>
                SetupNoExistingServicesOrGlobalConfig();

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
                            Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                                    p.CorrespondsToPluginConfig(targetPlugin) &&
                                    p.CorrespondsToKongService(targetService))), Times.Once);
                        }
                        foreach (var targetRoute in targetService.Routes)
                        {
                            foreach (var targetPlugin in targetRoute.Plugins)
                            {
                                Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                                    p.CorrespondsToPluginConfig(targetPlugin) &&
                                    p.CorrespondsToKongRoute(targetRoute))), Times.Once);
                            }
                        }
                    }
                    foreach (var targetPlugin in TargetGlobalConfig.Plugins)
                    {
                        Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                            p.CorrespondsToPluginConfig(targetPlugin) &&
                            p.IsGlobalPlugin())), Times.Once);
                    }
                }

                public void AndNoPluginsAreDeleted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(It.IsAny<string>()), Times.Never);
            }
        }

        public abstract class WithAnAssortmentOfExistingServicesAndGlobalConfig : WhenProcessingServicesAndGlobalConfigChanges
        {
            protected void GivenAnAssortmentOfExistingServicesAndGlobalConfig() =>
                SetupAnAssortmentOfExistingServicesAndGlobalConfig();

            public class AndNoTargetServicesOrGlobalConfig : WithAnAssortmentOfExistingServicesAndGlobalConfig
            {
                public void AndGivenNoTargetServicesOrGlobalConfig() =>
                    SetupNoTargetServicesOrGlobalConfig();

                public void ThenNoServicesAreAdded() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddService(It.IsAny<KongService>()), Times.Never);

                public void AndNoServicesAreUpdated() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpdateService(It.IsAny<KongService>()), Times.Never);

                public void AndTheExistingServicesAreDeleted()
                {
                    foreach (var existingService in ExistingServices)
                    {
                        Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteService(existingService.Id), Times.Once);
                    }
                }

                public void AndNoRoutesAreAdded() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Never);

                public void AndNoRoutesAreDeleted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteRoute(It.IsAny<string>()), Times.Never);

                public void AndNoPluginsAreUpserted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.IsAny<PluginBody>()), Times.Never);

                public void AndTheExistingPluginsAreDeleted()
                {
                    foreach (var existingPlugin in ExistingGlobalConfig.Plugins)
                    {
                        Mock.Get(The<IKongAdminWriter>())
                            .Verify(x => x.DeletePlugin(existingPlugin.id), Times.Once);
                    }
                }
            }

            public class AndAnAssortmentOfTargetServicesAndGlobalConfig : WithAnAssortmentOfExistingServicesAndGlobalConfig
            {
                public void AndGivenAnAssortmentOfTargetServicesAndGlobalConfig() =>
                    SetupAnAssortmentOfTargetServicesAndGlobalConfig();

                public void ThenTheNewServicesAreAdded() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddService(TargetServices[2]), Times.Once);

                public void AndTheChangedServicesAreUpdated() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpdateService(TargetServices[0]), Times.Once);

                public void AndTheUnchangedServicesAreNotUpdated() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpdateService(TargetServices[1]), Times.Never);

                public void AndTheRemovedServicesAreDeleted() =>
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteService(ExistingServices[2].Id), Times.Once);

                public void AndTheChangedOrNewRoutesAreAdded()
                {
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddRoute(TargetServices[0], TargetServices[0].Routes[0]), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddRoute(TargetServices[0], TargetServices[0].Routes[2]), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddRoute(TargetServices[1], TargetServices[1].Routes[0]), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddRoute(TargetServices[2], TargetServices[2].Routes[0]), Times.Once);
                }

                public void AndTheUnchangedRoutesAreNotAddedOrDeleted()
                {
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.AddRoute(TargetServices[0], TargetServices[0].Routes[1]), Times.Never);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteRoute(ExistingServices[0].Routes[1].Id), Times.Never);
                }

                public void AndChangedOrRemovedRoutesAreDeleted()
                {
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteRoute(ExistingServices[0].Routes[0].Id), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeleteRoute(ExistingServices[1].Routes[0].Id), Times.Once);
                }

                public void AndTheChangedOrNewPluginsAreUpserted()
                {
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[0].Routes[1].Plugins[0]) &&
                        p.CorrespondsToKongRoute(TargetServices[0].Routes[1]))), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[0].Routes[1].Plugins[2]) &&
                        p.CorrespondsToKongRoute(TargetServices[0].Routes[1]))), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[1].Plugins[0]) &&
                        p.CorrespondsToKongService(TargetServices[1]))), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[1].Plugins[2]) &&
                        p.CorrespondsToKongService(TargetServices[1]))), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[2].Routes[0].Plugins[0]) &&
                        p.CorrespondsToKongRoute(TargetServices[2].Routes[0]))), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[2].Routes[0].Plugins[1]) &&
                        p.CorrespondsToKongRoute(TargetServices[2].Routes[0]))), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[0].Plugins[0]) &&
                        p.CorrespondsToKongService(TargetServices[0]))), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[0].Plugins[1]) &&
                        p.CorrespondsToKongService(TargetServices[0]))), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetGlobalConfig.Plugins[0]) &&
                        p.IsGlobalPlugin())), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetGlobalConfig.Plugins[2]) &&
                        p.IsGlobalPlugin())), Times.Once);
                }

                public void AndTheUnchangedPluginsAreNotUpserted()
                {
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[0].Routes[1].Plugins[1]) &&
                        p.CorrespondsToKongRoute(TargetServices[0].Routes[1]))), Times.Never);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetServices[1].Plugins[1]) &&
                        p.CorrespondsToKongService(TargetServices[1]))), Times.Never);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(TargetGlobalConfig.Plugins[1]) &&
                        p.IsGlobalPlugin())), Times.Never);
                }

                public void AndNoneOfThePluginsOfChangedOrDeletedRoutesAreUpserted()
                {
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(ExistingServices[0].Routes[0].Plugins[0]) &&
                        p.CorrespondsToKongRoute(ExistingServices[0].Routes[0]))), Times.Never);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(ExistingServices[0].Routes[0].Plugins[1]) &&
                        p.CorrespondsToKongRoute(ExistingServices[0].Routes[0]))), Times.Never);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                        p.CorrespondsToPluginConfig(ExistingServices[0].Routes[0].Plugins[2]) &&
                        p.CorrespondsToKongRoute(ExistingServices[0].Routes[0]))), Times.Never);
                }

                public void AndTheRemovedPluginsAreDeleted()
                {
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(ExistingServices[0].Routes[1].Plugins[2].id), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(ExistingServices[1].Plugins[2].id), Times.Once);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(ExistingGlobalConfig.Plugins[2].id), Times.Once);
                }

                public void AndNoneOfThePluginsOfChangedOrDeletedRoutesAreDeleted()
                {
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(ExistingServices[0].Routes[0].Plugins[0].id), Times.Never);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(ExistingServices[0].Routes[0].Plugins[1].id), Times.Never);
                    Mock.Get(The<IKongAdminWriter>()).Verify(x => x.DeletePlugin(ExistingServices[0].Routes[0].Plugins[2].id), Times.Never);
                }
            }
        }
    }
}
