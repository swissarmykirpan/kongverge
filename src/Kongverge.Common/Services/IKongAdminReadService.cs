using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Services
{

    public interface IKongAdminReadService
    {
        Task<bool> KongIsReachable();
        Task<KongConfiguration> GetConfiguration();

        Task<List<KongService>> GetServices();
        Task<List<KongRoute>> GetRoutes();
        Task<List<KongRoute>> GetRoutes(string serviceName);
    }
}
