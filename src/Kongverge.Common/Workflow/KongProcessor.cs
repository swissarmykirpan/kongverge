using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Services;

namespace Kongverge.Common.Workflow
{
    public class KongProcessor
    {
        private readonly IKongAdminWriter _kongWriter;

        public KongProcessor(IKongAdminWriter kongWriter)
        {
            _kongWriter = kongWriter;
        }

        public async Task Process(KongvergeConfiguration existingConfiguration, KongvergeConfiguration targetConfiguration)
        {
            await ConvergeObjects(
                existingConfiguration.Services,
                targetConfiguration.Services,
                x => _kongWriter.DeleteService(x.Id),
                x => _kongWriter.AddService(x),
                x => _kongWriter.UpdateService(x),
                ConvergeServiceChildren).ConfigureAwait(false);

            await ConvergeChildrenPlugins(existingConfiguration.GlobalConfig, targetConfiguration.GlobalConfig).ConfigureAwait(false);
        }

        private static async Task ConvergeObjects<T>(
            IReadOnlyCollection<T> existingObjects,
            IReadOnlyCollection<T> targetObjects,
            Func<T, Task> deleteObject,
            Func<T, Task> createObject,
            Func<T, Task> updateObject = null,
            Func<T, T, Task> recurse = null) where T : KongObject
        {
            existingObjects = existingObjects ?? Array.Empty<T>();
            updateObject = updateObject ?? (x => Task.CompletedTask);
            recurse = recurse ?? ((e, t) => Task.CompletedTask);

            var targetMatchValues = targetObjects.Select(x => x.GetMatchValue()).ToArray();
            var toRemove = existingObjects.Where(x => !targetMatchValues.Contains(x.GetMatchValue()));

            foreach (var existing in toRemove)
            {
                await deleteObject(existing).ConfigureAwait(false);
            }

            foreach (var target in targetObjects)
            {
                var existing = target.MatchWithExisting(existingObjects);
                if (existing == null)
                {
                    await createObject(target).ConfigureAwait(false);
                }
                else if (!target.Equals(existing))
                {
                    await updateObject(target).ConfigureAwait(false);
                }
                await recurse(existing, target).ConfigureAwait(false);
            }
        }

        private Task ConvergeChildrenPlugins(ExtendibleKongObject existing, ExtendibleKongObject target)
        {
            Task UpsertPlugin(KongPlugin plugin, ExtendibleKongObject parent)
            {
                parent.AssignParentId(plugin);
                return _kongWriter.UpsertPlugin(plugin);
            }

            return ConvergeObjects(
                existing?.Plugins,
                target.Plugins,
                x => _kongWriter.DeletePlugin(x.Id),
                x => UpsertPlugin(x , target),
                x => UpsertPlugin(x, target));
        }

        private async Task ConvergeServiceChildren(KongService existing, KongService target)
        {
            await ConvergeChildrenPlugins(existing, target).ConfigureAwait(false);
            await ConvergeObjects(
                existing?.Routes,
                target.Routes,
                x => _kongWriter.DeleteRoute(x.Id),
                x => _kongWriter.AddRoute(target.Id, x),
                null,
                ConvergeChildrenPlugins).ConfigureAwait(false);
        }
    }
}
