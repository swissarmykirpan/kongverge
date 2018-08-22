using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Services
{
    public interface IKongAdminWriter
    {
        Task<KongService> AddService(KongService service);

        Task<KongService> UpdateService(KongService service);

        Task<string> DeleteService(string serviceId);

        Task<KongRoute> AddRoute(KongService service, KongRoute route);

        Task<IEnumerable<KongRoute>> DeleteRoutes(KongService service);

        Task<KongPluginResponse> UpsertPlugin(PluginBody plugin);

        Task DeleteRoute(string routeId);

        Task DeletePlugin(string pluginId);
    }
}
