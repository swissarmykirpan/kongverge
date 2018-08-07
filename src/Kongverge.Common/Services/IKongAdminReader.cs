using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Services
{

    public interface IKongAdminReader
    {
        Task<bool> KongIsReachable();

        Task<IReadOnlyCollection<KongService>> GetServices();
        Task<KongService> GetService(string serviceId);

        Task<IReadOnlyCollection<PluginBody>> GetServicePlugins(string serviceId);

        Task<KongAction<GlobalConfig>> GetGlobalConfig();
    }
}
