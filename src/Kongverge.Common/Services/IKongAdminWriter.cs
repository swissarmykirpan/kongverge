using System.Threading.Tasks;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Services
{
    public interface IKongAdminWriter
    {
        Task AddService(KongService service);

        Task UpdateService(KongService service);

        Task DeleteService(string serviceId);

        Task AddRoute(string serviceId, KongRoute route);

        Task DeleteRoute(string routeId);

        Task UpsertPlugin(KongPlugin plugin);

        Task DeletePlugin(string pluginId);
    }
}
