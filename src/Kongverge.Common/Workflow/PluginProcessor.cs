using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Workflow
{
    public class PluginProcessor
    {
        private readonly IKongAdminWriter _kongWriter;
        private readonly IKongPluginCollection _kongPluginCollection;

        public PluginProcessor(IKongAdminWriter kongWriter, IKongPluginCollection kongPluginCollection)
        {
            _kongWriter = kongWriter;
            _kongPluginCollection = kongPluginCollection;
        }

        public async Task Process(
            ExtendibleKongObject existing, ExtendibleKongObject target)
        {
            var newSet = PluginTypeMap(target?.Plugins);
            var existingSet = PluginTypeMap(existing?.Plugins);

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

        private static Dictionary<Type, IKongPluginConfig> PluginTypeMap(IList<IKongPluginConfig> plugins)
        {
            return plugins?.ToDictionary(e => e.GetType()) ?? new Dictionary<Type, IKongPluginConfig>();
        }
    }
}
