using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Serilog;

namespace Kongverge.Common.Workflow
{
    public class KongProcessor
    {
        private readonly IKongAdminWriter _kongWriter;
        private readonly PluginProcessor _pluginProcessor;

        public KongProcessor(IKongAdminWriter kongWriter)
        {
            _kongWriter = kongWriter;
            _pluginProcessor = new PluginProcessor(_kongWriter);
        }

        public async Task Process(KongvergeConfiguration existingConfiguration, KongvergeConfiguration targetConfiguration)
        {
            var existingServiceNames = existingConfiguration.Services.Select(x => x.Name).ToArray();
            var targetServiceNames = targetConfiguration.Services.Select(x => x.Name).ToArray();

            var toAdd = targetConfiguration.Services.Where(x => !existingServiceNames.Contains(x.Name));
            var toRemove = existingConfiguration.Services.Where(x => !targetServiceNames.Contains(x.Name));

            await Task.WhenAll(toRemove.Select(x => _kongWriter.DeleteService(x.Id))).ConfigureAwait(false);
            await Task.WhenAll(toAdd.Select(x => _kongWriter.AddService(x))).ConfigureAwait(false);

            foreach (var target in targetConfiguration.Services)
            {
                var existing = existingConfiguration.Services.SingleOrDefault(x => x.Name == target.Name);
                await ProcessService(existing, target).ConfigureAwait(false);
            }

            await _pluginProcessor.Process(existingConfiguration.GlobalConfig, targetConfiguration.GlobalConfig).ConfigureAwait(false);
        }

        private async Task ProcessService(KongService existing, KongService target)
        {
            if (ServiceHasChanged(existing, target))
            {
                await _kongWriter.UpdateService(target).ConfigureAwait(false);
            }

            if (existing != null)
            {
                target.Id = existing.Id;
            }

            Log.Information($"Processing plugins and routes for service {target.Name}");
            await _pluginProcessor.Process(existing, target).ConfigureAwait(false);
            await ConvergeRoutes(existing, target).ConfigureAwait(false);
        }

        private async Task ConvergeRoutes(KongService existing, KongService target)
        {
            var existingRoutes = existing?.Routes ?? new List<KongRoute>();

            var toAdd = target.Routes.Except(existingRoutes);
            var toRemove = existingRoutes.Except(target.Routes);

            await Task.WhenAll(toRemove.Select(r => _kongWriter.DeleteRoute(r.Id))).ConfigureAwait(false);
            await Task.WhenAll(toAdd.Select(r => _kongWriter.AddRoute(target.Id, r))).ConfigureAwait(false);

            foreach (var targetRoute in target.Routes)
            {
                var existingRoute = existingRoutes.SingleOrDefault(x => x.Equals(targetRoute));
                if (existingRoute != null)
                {
                    targetRoute.Id = existingRoute.Id;
                }

                await _pluginProcessor.Process(existingRoute, targetRoute).ConfigureAwait(false);
            }
        }

        private static bool ServiceHasChanged(KongService existing, KongService target)
        {
            if (existing == null || target.Equals(existing))
            {
                return false;
            }

            ServiceValidationHelper.PrintDiff(existing, target);
            return true;
        }
    }
}
