using System.Collections.Generic;
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

        public async Task Process<T>(T existing, T target) where T :  ExtendibleKongObject
        {
            var newSet = PluginNameMap(target.Plugins);
            var existingSet = PluginNameMap(existing?.Plugins);

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
                    Log.Information($"Deleting plugin {change.Existing.Name}");
                    await _kongWriter.DeletePlugin(change.Existing.Id).ConfigureAwait(false);
                }
                else if (change.Existing == null || !change.Target.Equals(change.Existing))
                {
                    if (existing != null && target.Id == null)
                    {
                        target.Id = existing.Id;
                    }

                    target.AssignParentId(change.Target);
                    change.Target.Id = change.Existing?.Id;
                    change.Target.CreatedAt = change.Existing?.CreatedAt;
                    await _kongWriter.UpsertPlugin(change.Target).ConfigureAwait(false);
                }
            }
        }

        private static Dictionary<string, KongPlugin> PluginNameMap(IEnumerable<KongPlugin> plugins)
        {
            return plugins?.ToDictionary(e => e.Name) ?? new Dictionary<string, KongPlugin>();
        }
    }
}
