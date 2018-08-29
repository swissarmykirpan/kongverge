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
            IReadOnlyCollection<KongService> existingServices, IReadOnlyCollection<KongService> newServices,
            GlobalConfig existingGlobalConfig, GlobalConfig newGlobalConfig)
        {
            await ProcessServices(existingServices, newServices).ConfigureAwait(false);

            await _pluginProcessor.Process(existingGlobalConfig, newGlobalConfig).ConfigureAwait(false);

            //Remove Missing Services
            var missingServices = existingServices
                .Except(newServices)
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

                await _kongWriter.AddService(newService).ConfigureAwait(false);

                await _pluginProcessor.Process(null, newService).ConfigureAwait(false);
                await ConvergeRoutes(null, newService).ConfigureAwait(false);
            }
            else
            {
                // TODO: Clean this up, its messy, but where else can we do this?
                newService.Id = existingService.Id;

                await _pluginProcessor.Process(existingService, newService).ConfigureAwait(false);

                await ConvergeRoutes(existingService, newService).ConfigureAwait(false);

                if (!ServiceHasChanged(existingService, newService))
                {
                    return;
                }

                Log.Information("Updating service: \"{name}\"", newService.Name);

                await _kongWriter.UpdateService(newService).ConfigureAwait(false);
            }
        }

        private async Task ConvergeRoutes(KongService existing, KongService target)
        {
            var existingRoutes = existing?.Routes ?? new List<KongRoute>();
            var toAdd = target.Routes.Except(existingRoutes);

            var toRemove = existingRoutes.Except(target.Routes);

            await Task.WhenAll(toRemove.Select(r => _kongWriter.DeleteRoute(r.Id))).ConfigureAwait(false);
            await Task.WhenAll(toAdd.Select(r => _kongWriter.AddRoute(target, r))).ConfigureAwait(false);

            var matchingRoutePairs =
                target.Routes.Select(r => new ExtendibleKongObjectTargetPair(r, existingRoutes));

            foreach (var routepair in matchingRoutePairs)
            {
                // TODO: Clean up same as before - the targets when loaded from file don't have IDs?
                //routepair.Target.Id = routepair.Existing.Id;

                await _pluginProcessor.Process(routepair.Existing, routepair.Target).ConfigureAwait(false);
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
