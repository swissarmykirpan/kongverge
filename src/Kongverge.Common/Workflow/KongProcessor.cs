using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Serilog;

namespace Kongverge.Common.Workflow
{
    public class KongProcessor
    {
        private readonly IKongAdminWriter _kongWriter;
        private readonly PluginProcessor _pluginProcessor;

        public KongProcessor(IKongAdminWriter kongWriter, IKongPluginCollection kongPluginCollection)
        {
            _kongWriter = kongWriter;
            _pluginProcessor = new PluginProcessor(_kongWriter, kongPluginCollection);
        }

        public async Task Process(
            IReadOnlyCollection<KongService> existingServices, IReadOnlyCollection<KongService> targetServices,
            GlobalConfig existingGlobalConfig, GlobalConfig targetGlobalConfig)
        {
            foreach (var target in targetServices)
            {
                await ProcessService(existingServices, target).ConfigureAwait(false);
            }

            await _pluginProcessor.Process(existingGlobalConfig, targetGlobalConfig).ConfigureAwait(false);

            //Remove Missing Services
            var missingServices = existingServices
                .Except(targetServices)
                .ToList();

            if (missingServices.Count > 0)
            {
                await DeleteServices(missingServices).ConfigureAwait(false);
            }
        }

        private async Task DeleteServices(IEnumerable<KongService> missingServices)
        {
            Log.Information("\nDeleting old services:");
            foreach (var service in missingServices)
            {
                Log.Information("Deleting service \"{serviceName}\"", service.Name);

                await _kongWriter.DeleteService(service.Id).ConfigureAwait(false);
            }
        }

        private async Task ProcessService(IEnumerable<KongService> existingServices, KongService target)
        {
            var existing = existingServices.SingleOrDefault(x => x.Name == target.Name);

            if (existing == null)
            {
                Log.Information("\nAdding new service: \"{name}\"", target.Name);

                var valid = await ServiceValidationHelper.Validate(target).ConfigureAwait(false);

                if (!valid)
                {
                    Log.Information("Invalid Data File: {name}{ext}", target.Name, Settings.FileExtension);
                    return;
                }

                await _kongWriter.AddService(target).ConfigureAwait(false);

                await _pluginProcessor.Process(null, target).ConfigureAwait(false);
                await ConvergeRoutes(null, target).ConfigureAwait(false);
            }
            else
            {
                // TODO: Clean this up, its messy, but where else can we do this?
                target.Id = existing.Id;

                await _pluginProcessor.Process(existing, target).ConfigureAwait(false);

                await ConvergeRoutes(existing, target).ConfigureAwait(false);

                if (!ServiceHasChanged(existing, target))
                {
                    return;
                }

                Log.Information("Updating service: \"{name}\"", target.Name);

                await _kongWriter.UpdateService(target).ConfigureAwait(false);
            }
        }

        private async Task ConvergeRoutes(KongService existing, KongService target)
        {
            var existingRoutes = existing?.Routes ?? new List<KongRoute>();
            var toAdd = target.Routes.Except(existingRoutes);

            // TODO: Consider detecting changes and patching existing routes, instead of deleting and adding again
            var toRemove = existingRoutes.Except(target.Routes);

            await Task.WhenAll(toRemove.Select(r => _kongWriter.DeleteRoute(r.Id))).ConfigureAwait(false);
            await Task.WhenAll(toAdd.Select(r => _kongWriter.AddRoute(target, r))).ConfigureAwait(false);

            var matchingRoutePairs = target.Routes.Select(r => new ExtendibleKongObjectTargetPair(r, existingRoutes));

            foreach (var routepair in matchingRoutePairs)
            {
                // TODO: Clean up same as before - the targets when loaded from file don't have IDs?
                //routepair.Target.Id = routepair.Existing.Id;

                await _pluginProcessor.Process(routepair.Existing, routepair.Target).ConfigureAwait(false);
            }
        }

        private static bool ServiceHasChanged(KongService existing, KongService target)
        {
            if (!existing.Equals(target))
            {
                ServiceValidationHelper.PrintDiff(existing, target);
                return true;
            }

            return false;
        }
    }
}
