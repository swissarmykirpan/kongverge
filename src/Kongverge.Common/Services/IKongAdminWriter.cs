using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Services
{
    public interface IKongAdminWriter
    {
        Task<KongAction<KongService>> AddService(KongService service);

        Task<KongAction<KongService>> UpdateService(KongService service);

        Task<KongAction<string>> DeleteService(string serviceId);

        Task<KongAction<KongRoute>> AddRoute(KongService service, KongRoute route);

        Task<KongAction<IEnumerable<KongRoute>>> DeleteRoutes(KongService service);

        Task<KongPluginResponse> UpsertPlugin(PluginBody plugin);

        Task<bool> DeleteRoute(string routeId);

        Task<bool> DeletePlugin(string pluginId);
    }
}
