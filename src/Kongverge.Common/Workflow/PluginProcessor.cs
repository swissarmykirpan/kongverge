using System;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Services;
using Serilog;

namespace Kongverge.Common.Workflow
{
    public class PluginProcessor
    {
        private readonly IKongAdminWriter _kongWriter;

        public PluginProcessor(IKongAdminWriter kongWriter)
        {
            _kongWriter = kongWriter;
        }

        public async Task Process<T>(T existing, T target) where T : ExtendibleKongObject
        {
            var existingPlugins = existing?.Plugins ?? Array.Empty<KongPlugin>();

            var targetPluginNames = target.Plugins.Select(x => x.Name).ToArray();
            var toRemove = existingPlugins.Where(x => !targetPluginNames.Contains(x.Name));

            foreach (var existingPlugin in toRemove)
            {
                Log.Information($"Deleting plugin {existingPlugin.Name}");
                await _kongWriter.DeletePlugin(existingPlugin.Id).ConfigureAwait(false);
            }

            foreach (var targetPlugin in target.Plugins)
            {
                var existingPlugin = targetPlugin.MatchWithExisting(existingPlugins);
                if (!targetPlugin.Equals(existingPlugin))
                {
                    target.AssignParentId(targetPlugin);
                    await _kongWriter.UpsertPlugin(targetPlugin).ConfigureAwait(false);
                }
            }
        }
    }
}
