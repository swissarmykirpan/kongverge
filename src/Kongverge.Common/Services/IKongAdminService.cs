using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Services
{
    public interface IKongAdminService : IKongAdminReadService
    {
        Task<KongAction<KongService>> AddService(KongService service);

        Task<KongAction<KongService>> UpdateService(KongService service);

        Task<KongAction<KongService>> DeleteService(KongService service);

        Task<KongAction<KongRoute>> AddRoute(KongService service, KongRoute route);

        Task<KongAction<IEnumerable<KongRoute>>> DeleteRoutes(KongService service);

        Task<KongPluginResponse> UpsertPlugin(PluginBody plugin);

        Task<bool> DeleteRoute(KongRoute route);

        Task<bool> DeletePlugin(string pluginId);
    }
}
