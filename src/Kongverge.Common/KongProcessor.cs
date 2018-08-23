using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Kongverge.KongPlugin;
using Serilog;

namespace Kongverge.Common
{
    public class KongProcessor
    {
        private readonly IKongAdminWriter _kongWriter;
        private readonly IKongPluginCollection _kongPluginCollection;

        public KongProcessor(IKongAdminWriter kongWriter, IKongPluginCollection kongPluginCollection)
        {
            _kongWriter = kongWriter;
            _kongPluginCollection = kongPluginCollection;
        }

        public async Task Process(
            IReadOnlyCollection<KongService> existingServices, List<KongService> newServices,
            GlobalConfig existingConfig, GlobalConfig newConfig)
        {
            await ProcessServices(existingServices, newServices).ConfigureAwait(false);

            await ConvergePlugins(newConfig, existingConfig);

            //Remove Missing Services
            var missingServices = existingServices
                .Except(newServices)
                .ToList();

            if (missingServices.Count > 0)
            {
                await DeleteServicesMissingFromConfig(missingServices).ConfigureAwait(false);
            }
        }

        private async Task DeleteServicesMissingFromConfig(IEnumerable<KongService> missingServices)
        {
            Log.Information("\nDeleting old services:");
            foreach (var service in missingServices)
            {
                Log.Information("Deleting service \"{serviceName}\"", service.Name);

                await _kongWriter.DeleteRoutes(service).ConfigureAwait(false);
                await _kongWriter.DeleteService(service.Id).ConfigureAwait(false);
            }
        }

        private async Task ProcessServices(
            IReadOnlyCollection<KongService> existingServices,
            IReadOnlyCollection<KongService> newServices)
        {
            foreach (var service in newServices)
            {
                await ProcessService(existingServices, service).ConfigureAwait(false);
            }
        }

        private async Task ProcessService(IEnumerable<KongService> existingServices, KongService newService)
        {
            var existingService = existingServices.SingleOrDefault(x => x.Name == newService.Name);

            if (existingService == null)
            {
                Log.Information("\nAdding new service: \"{name}\"", newService.Name);

                var valid = await ServiceValidationHelper.Validate(newService).ConfigureAwait(false);

                if (!valid)
                {
                    Log.Information("Invalid Data File: {name}{ext}", newService.Name, Settings.FileExtension);
                    return;
                }

                var serviceAdded = await _kongWriter.AddService(newService).ConfigureAwait(false);

                await ConvergePlugins(newService, serviceAdded).ConfigureAwait(false);
                await ConvergeRoutes(newService, serviceAdded).ConfigureAwait(false);
            }
            else
            {
                // TODO: Clean this up, its messy, but where else can we do this?
                newService.Id = existingService.Id;

                await ConvergePlugins(newService, existingService).ConfigureAwait(false);

                await ConvergeRoutes(newService, existingService).ConfigureAwait(false);

                if (!ServiceHasChanged(existingService, newService))
                {
                    return;
                }

                Log.Information("Updating service: \"{name}\"", newService.Name);

                await _kongWriter.UpdateService(newService).ConfigureAwait(false);
            }
        }

        public async Task ConvergeRoutes(KongService target, KongService existing)
        {
            var toAdd = target.Routes.Except(existing.Routes);
            var toRemove = existing.Routes.Except(target.Routes);

            await Task.WhenAll(toRemove.Select(r => _kongWriter.DeleteRoute(r.Id))).ConfigureAwait(false);
            await Task.WhenAll(toAdd.Select(r => _kongWriter.AddRoute(target, r))).ConfigureAwait(false);

            var matchingRoutePairs =
                target.Routes.Select(r => new ExtendibleKongObjectTargetPair(r, existing.Routes));

            foreach (var routepair in matchingRoutePairs)
            {
                // TODO: Clean up same as before - the targets when loaded from file don't have IDs?
                //routepair.Target.Id = routepair.Existing.Id;

                await ConvergePlugins(routepair.Target, routepair.Existing).ConfigureAwait(false);
            }
        }

        public async Task ConvergePlugins(ExtendibleKongObject target, ExtendibleKongObject existing)
        {
            var newSet = target?.Plugins?.ToDictionary(e => e.GetType()) ?? new Dictionary<Type, IKongPluginConfig>();
            var existingSet = existing?.Plugins?.ToDictionary(t => t.GetType()) ?? new Dictionary<Type, IKongPluginConfig>();

            var changes = newSet.Keys.Union(existingSet.Keys)
                .Select(key =>
                    new
                    {
                        Target = newSet.ContainsKey(key) ? newSet[key] : null,
                        Existing = existingSet.ContainsKey(key) ? existingSet[key] : null
                    });

            foreach (var change in changes)
            {
                if (change.Target == null)
                {
                    await _kongWriter.DeletePlugin(change.Existing.id).ConfigureAwait(false);
                }
                else if (change.Existing == null)
                {
                    var content = _kongPluginCollection.CreatePluginBody(change.Target);

                    await _kongWriter.UpsertPlugin(target.DecoratePluginBody(content)).ConfigureAwait(false);
                }
                else if(!change.Target.IsExactMatch(change.Existing))
                {
                    var content = _kongPluginCollection.CreatePluginBody(change.Target);

                    content.id = change.Existing.id;

                    // TODO: Same problem here - target has come from a file, and it doesn't have the Created info to feed into created_at
                    target.Created = existing.Created;

                    await _kongWriter.UpsertPlugin(target.DecoratePluginBody(content)).ConfigureAwait(false);
                }
            }
        }

        private static bool ServiceHasChanged(KongService existingService, KongService newService)
        {
            if (!existingService.Equals(newService))
            {
                ServiceValidationHelper.PrintDiff(existingService, newService);
                return true;
            }

            return false;
        }
    }
}