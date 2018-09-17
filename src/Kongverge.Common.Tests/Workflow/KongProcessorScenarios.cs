using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Kongverge.Common.DTOs;
using Kongverge.Common.Services;
using Kongverge.Common.Workflow;
using Kongverge.TestHelpers;
using Moq;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace Kongverge.Common.Tests.Workflow
{
    public class KongProcessorScenarios : ScenarioFor<KongProcessor>
    {
        protected const string And = "_";

        protected IReadOnlyList<KongService> ExistingServices;
        protected ExtendibleKongObject ExistingGlobalConfig;
        protected IReadOnlyList<KongService> TargetServices;
        protected ExtendibleKongObject TargetGlobalConfig;

        protected IReadOnlyList<ExtendibleKongObject> GlobalConfigs;
        protected IReadOnlyList<KongService> Services;
        protected IReadOnlyList<KongRoute> Routes;
        protected IReadOnlyList<KongPlugin> Plugins;

        protected List<KongPlugin> InsertedPlugins = new List<KongPlugin>();

        public KongProcessorScenarios()
        {
            var fixture = new Fixture();
            GlobalConfigs = fixture.CreateGlobalConfigs(2);
            Services = fixture.CreateServices(4);
            Routes = fixture.CreateRoutes(6);
            Plugins = fixture.CreatePlugins(10);
            
            GetMock<IKongAdminWriter>()
                .Setup(x => x.AddRoute(It.IsAny<string>(), It.IsAny<KongRoute>()))
                .Returns<string, KongRoute>((serviceId, route) =>
                {
                    if (serviceId == null)
                    {
                        throw new InvalidOperationException();
                    }
                    route.WithIdAndCreatedAt();
                    return Task.CompletedTask;
                });

            GetMock<IKongAdminWriter>()
                .Setup(x => x.UpsertPlugin(It.IsAny<KongPlugin>()))
                .Returns<KongPlugin>(x =>
                {
                    if (x.Id == null)
                    {
                        InsertedPlugins.Add(x);
                    }
                    x.WithIdAndCreatedAt();
                    return Task.CompletedTask;
                });

            GetMock<IKongAdminWriter>()
                .Setup(x => x.AddService(It.IsAny<KongService>()))
                .Returns<KongService>(x =>
                {
                    x.WithIdAndCreatedAt();
                    return Task.CompletedTask;
                });
        }

        [BddfyFact(DisplayName = nameof(NoExistingServicesOrGlobalConfig) + And + nameof(NoTargetServicesOrGlobalConfig))]
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

        [BddfyFact(DisplayName = nameof(NoExistingServicesOrGlobalConfig) + And + nameof(AnAssortmentOfTargetServicesAndGlobalConfig))]
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

        [BddfyFact(DisplayName = nameof(AnAssortmentOfExistingServicesAndGlobalConfig) + And + nameof(NoTargetServicesOrGlobalConfig))]
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

        [BddfyFact(DisplayName = nameof(AnAssortmentOfExistingServicesAndGlobalConfig) + And + nameof(AnAssortmentOfTargetServicesAndGlobalConfig))]
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
                .And(s => s.NoneOfThePluginsOfDeletedServicesAreDeleted())
                .And(s => s.NoneOfThePluginsOfChangedOrDeletedRoutesAreDeleted())
                .BDDfy();

        protected void NoExistingServicesOrGlobalConfig()
        {
            ExistingServices = Array.Empty<KongService>();
            ExistingGlobalConfig = new ExtendibleKongObject();
        }

        protected void AnAssortmentOfExistingServicesAndGlobalConfig()
        {
            ExistingServices = new[]
            {
                Services[0].AsExisting(),
                Services[1].AsExisting(),
                Services[2].AsExisting()
            };

            ExistingServices[0].Plugins = new[]
            {
                Plugins[3].AsExisting(),
                Plugins[4].AsExisting(),
                Plugins[5].AsExisting()
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

            ExistingServices[2].Plugins = new[]
            {
                Plugins[0].AsExisting()
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
            TargetGlobalConfig = new ExtendibleKongObject();
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

            TargetServices[0].Plugins = new[]
            {
                Plugins[3].AsTarget(true), // Changed
                Plugins[4].AsTarget(), // Same
                // Plugins[5] Removed
                Plugins[6].AsTarget() // Added
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

            TargetServices[2].Routes = new[]
            {
                Routes[5].AsTarget() // Added
            };
            TargetServices[2].Routes[0].Plugins = new[]
            {
                Plugins[0].AsTarget(), // Added
                Plugins[1].AsTarget() // Added
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

        protected async Task InvokingProcess()
        {
            var existingConfiguration = new KongvergeConfiguration
            {
                Services = ExistingServices,
                GlobalConfig = ExistingGlobalConfig
            };
            var targetConfiguration = new KongvergeConfiguration
            {
                Services = TargetServices,
                GlobalConfig = TargetGlobalConfig
            };
            await Subject.Process(existingConfiguration, targetConfiguration);
        }

        protected void NoServicesAreAdded() =>
            GetMock<IKongAdminWriter>().Verify(x => x.AddService(It.IsAny<KongService>()), Times.Never);

        protected void NoServicesAreUpdated() =>
            GetMock<IKongAdminWriter>().Verify(x => x.UpdateService(It.IsAny<KongService>()), Times.Never);

        protected void NoServicesAreDeleted() =>
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteService(It.IsAny<string>()), Times.Never);

        protected void NoRoutesAreAdded() =>
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(It.IsAny<string>(), It.IsAny<KongRoute>()), Times.Never);

        protected void NoRoutesAreDeleted() =>
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteRoute(It.IsAny<string>()), Times.Never);

        protected void NoPluginsAreUpserted() =>
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.IsAny<KongPlugin>()), Times.Never);

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
                    GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(targetService.Id, targetRoute), Times.Once);
                }
            }
        }

        protected void TheTargetPluginsAreUpserted()
        {
            foreach (var targetService in TargetServices)
            {
                foreach (var targetPlugin in targetService.Plugins)
                {
                    GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                        p.IsTheSameAs(targetPlugin) &&
                        p.CorrespondsToKongService(targetService))), Times.Once);
                }
                foreach (var targetRoute in targetService.Routes)
                {
                    foreach (var targetPlugin in targetRoute.Plugins)
                    {
                        GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                            p.IsTheSameAs(targetPlugin) &&
                            p.CorrespondsToKongRoute(targetRoute))), Times.Once);
                    }
                }
            }
            foreach (var targetPlugin in TargetGlobalConfig.Plugins)
            {
                GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                    p.IsTheSameAs(targetPlugin) &&
                    p.IsGlobal())), Times.Once);
            }
        }

        protected void TheExistingPluginsAreDeleted()
        {
            foreach (var existingPlugin in ExistingGlobalConfig.Plugins)
            {
                GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(existingPlugin.Id), Times.Once);
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
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(TargetServices[0].Id, TargetServices[0].Routes[0]), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(TargetServices[0].Id, TargetServices[0].Routes[2]), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(ExistingServices[1].Id, TargetServices[1].Routes[0]), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(TargetServices[2].Id, TargetServices[2].Routes[0]), Times.Once);
        }

        protected void TheUnchangedRoutesAreNotAddedOrDeleted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.AddRoute(TargetServices[0].Id, TargetServices[0].Routes[1]), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteRoute(ExistingServices[0].Routes[1].Id), Times.Never);
        }

        protected void TheChangedOrRemovedRoutesAreDeleted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteRoute(ExistingServices[0].Routes[0].Id), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.DeleteRoute(ExistingServices[1].Routes[0].Id), Times.Once);
        }

        protected void TheChangedOrNewPluginsAreUpserted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetServices[0].Plugins[0]) &&
                p.CorrespondsToKongService(TargetServices[0]) &&
                p.CorrespondsToExistingPlugin(ExistingServices[0].Plugins[0]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetServices[0].Plugins[2]) &&
                p.CorrespondsToKongService(TargetServices[0]) &&
                InsertedPlugins.Contains(p))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetServices[0].Routes[1].Plugins[0]) &&
                p.CorrespondsToKongRoute(TargetServices[0].Routes[1]) &&
                p.CorrespondsToExistingPlugin(ExistingServices[0].Routes[1].Plugins[0]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetServices[0].Routes[1].Plugins[2]) &&
                p.CorrespondsToKongRoute(TargetServices[0].Routes[1]) &&
                InsertedPlugins.Contains(p))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetServices[2].Routes[0].Plugins[0]) &&
                p.CorrespondsToKongRoute(TargetServices[2].Routes[0]) &&
                InsertedPlugins.Contains(p))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetServices[2].Routes[0].Plugins[1]) &&
                p.CorrespondsToKongRoute(TargetServices[2].Routes[0]) &&
                InsertedPlugins.Contains(p))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetGlobalConfig.Plugins[0]) &&
                p.IsGlobal() &&
                p.CorrespondsToExistingPlugin(ExistingGlobalConfig.Plugins[0]))), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetGlobalConfig.Plugins[2]) &&
                p.IsGlobal() &&
                InsertedPlugins.Contains(p))), Times.Once);
        }

        protected void TheUnchangedPluginsAreNotUpserted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetServices[0].Routes[1].Plugins[1]) &&
                p.CorrespondsToKongRoute(TargetServices[0].Routes[1]))), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetServices[0].Plugins[1]) &&
                p.CorrespondsToKongService(TargetServices[0]))), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(TargetGlobalConfig.Plugins[1]) &&
                p.IsGlobal())), Times.Never);
        }

        protected void NoneOfThePluginsOfChangedOrDeletedRoutesAreUpserted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(ExistingServices[0].Routes[0].Plugins[0]) &&
                p.CorrespondsToKongRoute(ExistingServices[0].Routes[0]))), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(ExistingServices[0].Routes[0].Plugins[1]) &&
                p.CorrespondsToKongRoute(ExistingServices[0].Routes[0]))), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.UpsertPlugin(It.Is<KongPlugin>(p =>
                p.IsTheSameAs(ExistingServices[0].Routes[0].Plugins[2]) &&
                p.CorrespondsToKongRoute(ExistingServices[0].Routes[0]))), Times.Never);
        }

        protected void TheRemovedPluginsAreDeleted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[0].Plugins[2].Id), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[0].Routes[1].Plugins[2].Id), Times.Once);
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingGlobalConfig.Plugins[2].Id), Times.Once);
        }

        protected void NoneOfThePluginsOfDeletedServicesAreDeleted() =>
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[2].Plugins[0].Id), Times.Never);

        protected void NoneOfThePluginsOfChangedOrDeletedRoutesAreDeleted()
        {
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[0].Routes[0].Plugins[0].Id), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[0].Routes[0].Plugins[1].Id), Times.Never);
            GetMock<IKongAdminWriter>().Verify(x => x.DeletePlugin(ExistingServices[0].Routes[0].Plugins[2].Id), Times.Never);
        }
    }
}
