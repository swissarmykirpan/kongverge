using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Services
{
    public interface IKongAdminWriter
    {
        Task AddService(KongService service);

        Task UpdateService(KongService service);

        Task DeleteService(string serviceId);

        Task AddRoute(KongService service, KongRoute route);

        Task DeleteRoutes(KongService service);

        Task UpsertPlugin(PluginBody plugin);

        Task DeleteRoute(string routeId);

        Task DeletePlugin(string pluginId);
    }
}
