using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Services
{

    public interface IKongAdminReader
    {
        Task<bool> KongIsReachable();

        Task<KongConfiguration> GetConfiguration();

        Task<IReadOnlyCollection<KongService>> GetServices();
        Task<KongService> GetService(string serviceId);

        Task<GlobalConfig> GetGlobalConfig();
    }
}
