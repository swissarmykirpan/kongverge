using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Services
{
    public interface IKongAdminReader
    {
        Task<bool> KongIsReachable();
        Task<KongConfiguration> GetConfiguration();
        Task<KongService> GetService(string serviceId);
        Task<IReadOnlyCollection<KongService>> GetServices();
        Task<IReadOnlyCollection<KongRoute>> GetRoutes();
        Task<IReadOnlyCollection<KongPlugin>> GetPlugins();
    }
}
