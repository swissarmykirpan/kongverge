using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Kongverge.Common.Workflow;
using Kongverge.KongPlugin;
using Kongverge.TestHelpers;
using Moq;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace Kongverge.Common.Tests.Workflow
{
    public class KongProcessorScenarios : ScenarioFor<KongProcessor>
    {
        protected const string Plus = "+";

        protected IReadOnlyList<KongService> ExistingServices;
        protected GlobalConfig ExistingGlobalConfig;
        protected IReadOnlyList<KongService> TargetServices;
        protected GlobalConfig TargetGlobalConfig;

        protected IReadOnlyList<GlobalConfig> GlobalConfigs;
        protected IReadOnlyList<KongService> Services;
        protected IReadOnlyList<KongRoute> Routes;
        protected IReadOnlyList<TestPlugins.FakePluginConfig> Plugins;

        public KongProcessorScenarios()
        {
            var fixture = new Fixture();
            GlobalConfigs = fixture.CreateGlobalConfigs(2);
            Services = fixture.CreateServices(4);
            Routes = fixture.CreateRoutes(6);
            Plugins = fixture.CreatePlugins(10);

            GetMock<IKongPluginCollection>()
                .Setup(x => x.CreatePluginBody(It.IsAny<TestPlugins.FakePluginConfig>()))
                .Returns<TestPlugins.FakePluginConfig>(x => x.CreatePluginBody());

            GetMock<IKongAdminWriter>()
                .Setup(x => x.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()))
                .Returns<KongService, KongRoute>((service, route) =>
                {
                    route.WithId();
                    return Task.CompletedTask;
                });

            GetMock<IKongAdminWriter>()
                .Setup(x => x.UpsertPlugin(It.IsAny<PluginBody>()))
                .Returns<PluginBody>(x =>
                {
                    x.WithId();
                    return Task.CompletedTask;
                });

            GetMock<IKongAdminWriter>()
                .Setup(x => x.AddService(It.IsAny<KongService>()))
                .Returns<KongService>(x =>
                {
                    x.WithId();
                    return Task.CompletedTask;
                });
        }

        [BddfyFact(DisplayName = nameof(NoExistingServicesOrGlobalConfig) + Plus + nameof(NoTargetServicesOrGlobalConfig))]
        public void Scenario1() =>
            this.Given(s => s.NoExistingServicesOrGlobalConfig())
                .And(s => s.NoTargetServicesOrGlobalConfig())
                .When(s => s.InvokingProcess())
                .Then(s => s.NoServicesAreAdded())
                .And(s => s.NoServicesAreUpdated())
                .And(s => s.NoServicesAreDeleted())
                .And(s => s.NoRoutesAreAdded())
                .And(s => s.NoRoutesAreDeleted())
                .And(s => s.NoPluginsAreUpserted())
                .And(s => s.NoPluginsAreDeleted())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(NoExistingServicesOrGlobalConfig) + Plus + nameof(AnAssortmentOfTargetServicesAndGlobalConfig))]
        public void Scenario2() =>
            this.Given(s => s.NoExistingServicesOrGlobalConfig())
                .And(s => s.AnAssortmentOfTargetServicesAndGlobalConfig())
                .When(s => s.InvokingProcess())
                .Then(s => s.TheTargetServicesAreAdded())
                .And(s => s.NoServicesAreUpdated())
                .And(s => s.NoServicesAreDeleted())
                .And(s => s.TheTargetRoutesAreAdded())
                .And(s => s.NoRoutesAreDeleted())
                .And(s => s.TheTargetPluginsAreUpserted())
                .And(s => s.NoPluginsAreDeleted())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AnAssortmentOfExistingServicesAndGlobalConfig) + Plus + nameof(NoTargetServicesOrGlobalConfig))]
        public void Scenario3() =>
            this.Given(s => s.AnAssortmentOfExistingServicesAndGlobalConfig())
                .And(s => s.NoTargetServicesOrGlobalConfig())
                .When(s => s.InvokingProcess())
                .Then(s => s.NoServicesAreAdded())
                .And(s => s.NoServicesAreUpdated())
                .And(s => s.TheExistingServicesAreDeleted())
                .And(s => s.NoRoutesAreAdded())
                .And(s => s.NoRoutesAreDeleted())
                .And(s => s.NoPluginsAreUpserted())
                .And(s => s.TheExistingPluginsAreDeleted())
                .BDDfy();

        [BddfyFact(DisplayName = nameof(AnAssortmentOfExistingServicesAndGlobalConfig) + Plus + nameof(AnAssortmentOfTargetServicesAndGlobalConfig))]
        public void Scenario4() =>
            this.Given(s => s.AnAssortmentOfExistingServicesAndGlobalConfig())
                .And(s => s.AnAssortmentOfTargetServicesAndGlobalConfig())
                .When(s => s.InvokingProcess())
                .Then(s => s.TheNewServicesAreAdded())
                .And(s => s.TheChangedServicesAreUpdated())
                .And(s => s.TheUnchangedServicesAreNotUpdated())
                .And(s => s.TheRemovedServicesAreDeleted())
                .And(s => s.TheChangedOrNewRoutesAreAdded())
                .And(s => s.TheUnchangedRoutesAreNotAddedOrDeleted())
                .And(s => s.TheChangedOrRemovedRoutesAreDeleted())
                .And(s => s.TheChangedOrNewPluginsAreUpserted())
                .And(s => s.TheUnchangedPluginsAreNotUpserted())
                .And(s => s.NoneOfThePluginsOfChangedOrDeletedRoutesAreUpserted())
                .And(s => s.TheRemovedPluginsAreDeleted())
                .And(s => s.NoneOfThePluginsOfChangedOrDeletedRoutesAreDeleted())
                .BDDfy();

        protected void NoExistingServicesOrGlobalConfig()
        {
            ExistingServices = Array.Empty<KongService>();
            ExistingGlobalConfig = null;
        }

        protected void AnAssortmentOfExistingServicesAndGlobalConfig()
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

        protected void NoTargetServicesOrGlobalConfig()
        {
            TargetServices = Array.Empty<KongService>();
            TargetGlobalConfig = null;
        }

        protected void AnAssortmentOfTargetServicesAndGlobalConfig()
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

        protected async Task InvokingProcess() =>
            await Subject.Process(ExistingServices, TargetServices, ExistingGlobalConfig, TargetGlobalConfig);
        
        protected void NoServicesAreAdded() =>
            GetMock<IKongAdminWriter>().Verify(x => x.AddService(It.IsAny<KongService>()), Times.Never);

        protected void NoServicesAreUpdated() =>
            GetMock<IKongAdminWriter>().Verify(x => x.UpdateService(It.IsAny<KongService>()), Times.Never);

        protected void NoServicesAreDeleted() =>
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteService(It.IsAny<string>()), Times.Never);

        protected void NoRoutesAreAdded() =>
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Never);

        protected void NoRoutesAreDeleted() =>
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteRoute(It.IsAny<string>()), Times.Never);

        protected void NoPluginsAreUpserted() =>
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.IsAny<PluginBody>()), Times.Never);

        protected void NoPluginsAreDeleted() =>
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(It.IsAny<string>()), Times.Never);

        protected void TheTargetServicesAreAdded()
        {
            foreach (var targetService in TargetServices)
            {
                GetMock<IKongAdminWriter>().Verify(x => x.AddService(targetService), Times.Once);
            }
        }

        protected void TheExistingServicesAreDeleted()
        {
            foreach (var existingService in ExistingServices)
            {
                GetMock<IKongAdminWriter>().Verify(x => x.DeleteService(existingService.Id), Times.Once);
            }
        }

        protected void TheTargetRoutesAreAdded()
        {
            foreach (var targetService in TargetServices)
            {
                foreach (var targetRoute in targetService.Routes)
                {
                    GetMock<IKongAdminWriter>()
                        .Verify(x => x.AddRoute(targetService, targetRoute), Times.Once);
                }
            }
        }

        protected void TheTargetPluginsAreUpserted()
        {
            foreach (var targetService in TargetServices)
            {
                foreach (var targetPlugin in targetService.Plugins)
                {
                    GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                            p.CorrespondsToPluginConfig(targetPlugin) &&
                            p.CorrespondsToKongService(targetService))), Times.Once);
                }
                foreach (var targetRoute in targetService.Routes)
                {
                    foreach (var targetPlugin in targetRoute.Plugins)
                    {
                        GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                            p.CorrespondsToPluginConfig(targetPlugin) &&
                            p.CorrespondsToKongRoute(targetRoute))), Times.Once);
                    }
                }
            }
            foreach (var targetPlugin in TargetGlobalConfig.Plugins)
            {
                GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                    p.CorrespondsToPluginConfig(targetPlugin) &&
                    p.IsGlobalPlugin())), Times.Once);
            }
        }

        protected void TheExistingPluginsAreDeleted()
        {
            foreach (var existingPlugin in ExistingGlobalConfig.Plugins)
            {
                GetMock<IKongAdminWriter>()
                    .Verify(x => x.DeletePlugin(existingPlugin.id), Times.Once);
            }
        }

        protected void TheNewServicesAreAdded() =>
            GetMock<IKongAdminWriter>().Verify(x => x.AddService(TargetServices[2]), Times.Once);

        protected void TheChangedServicesAreUpdated() =>
            GetMock<IKongAdminWriter>().Verify(x => x.UpdateService(TargetServices[0]), Times.Once);

        protected void TheUnchangedServicesAreNotUpdated() =>
            GetMock<IKongAdminWriter>().Verify(x => x.UpdateService(TargetServices[1]), Times.Never);

        protected void TheRemovedServicesAreDeleted() =>
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteService(ExistingServices[2].Id), Times.Once);

        protected void TheChangedOrNewRoutesAreAdded()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(TargetServices[0], TargetServices[0].Routes[0]), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(TargetServices[0], TargetServices[0].Routes[2]), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(TargetServices[1], TargetServices[1].Routes[0]), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(TargetServices[2], TargetServices[2].Routes[0]), Times.Once);
        }

        protected void TheUnchangedRoutesAreNotAddedOrDeleted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(TargetServices[0], TargetServices[0].Routes[1]), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteRoute(ExistingServices[0].Routes[1].Id), Times.Never);
        }

        protected void TheChangedOrRemovedRoutesAreDeleted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteRoute(ExistingServices[0].Routes[0].Id), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteRoute(ExistingServices[1].Routes[0].Id), Times.Once);
        }

        protected void TheChangedOrNewPluginsAreUpserted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[0].Routes[1].Plugins[0]) &&
                p.CorrespondsToKongRoute(TargetServices[0].Routes[1]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[0].Routes[1].Plugins[2]) &&
                p.CorrespondsToKongRoute(TargetServices[0].Routes[1]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[1].Plugins[0]) &&
                p.CorrespondsToKongService(TargetServices[1]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[1].Plugins[2]) &&
                p.CorrespondsToKongService(TargetServices[1]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[2].Routes[0].Plugins[0]) &&
                p.CorrespondsToKongRoute(TargetServices[2].Routes[0]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[2].Routes[0].Plugins[1]) &&
                p.CorrespondsToKongRoute(TargetServices[2].Routes[0]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[0].Plugins[0]) &&
                p.CorrespondsToKongService(TargetServices[0]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[0].Plugins[1]) &&
                p.CorrespondsToKongService(TargetServices[0]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetGlobalConfig.Plugins[0]) &&
                p.IsGlobalPlugin())), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetGlobalConfig.Plugins[2]) &&
                p.IsGlobalPlugin())), Times.Once);
        }

        protected void TheUnchangedPluginsAreNotUpserted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[0].Routes[1].Plugins[1]) &&
                p.CorrespondsToKongRoute(TargetServices[0].Routes[1]))), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetServices[1].Plugins[1]) &&
                p.CorrespondsToKongService(TargetServices[1]))), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(TargetGlobalConfig.Plugins[1]) &&
                p.IsGlobalPlugin())), Times.Never);
        }

        protected void NoneOfThePluginsOfChangedOrDeletedRoutesAreUpserted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(ExistingServices[0].Routes[0].Plugins[0]) &&
                p.CorrespondsToKongRoute(ExistingServices[0].Routes[0]))), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(ExistingServices[0].Routes[0].Plugins[1]) &&
                p.CorrespondsToKongRoute(ExistingServices[0].Routes[0]))), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<PluginBody>(p =>
                p.CorrespondsToPluginConfig(ExistingServices[0].Routes[0].Plugins[2]) &&
                p.CorrespondsToKongRoute(ExistingServices[0].Routes[0]))), Times.Never);
        }

        protected void TheRemovedPluginsAreDeleted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[0].Routes[1].Plugins[2].id), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[1].Plugins[2].id), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingGlobalConfig.Plugins[2].id), Times.Once);
        }

        protected void NoneOfThePluginsOfChangedOrDeletedRoutesAreDeleted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[0].Routes[0].Plugins[0].id), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[0].Routes[0].Plugins[1].id), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[0].Routes[0].Plugins[2].id), Times.Never);
        }
    }
}
