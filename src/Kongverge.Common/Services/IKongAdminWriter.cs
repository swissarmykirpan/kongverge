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

        Task DeleteRoute(string routeId);

        Task UpsertPlugin(PluginBody plugin);

        Task DeletePlugin(string pluginId);
    }
}
