using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Services
{

    public interface IKongAdminReader
    {
        Task<bool> KongIsReachable();

        Task<IReadOnlyCollection<KongService>> GetServices();

        Task<KongAction<GlobalConfig>> GetGlobalConfig();
    }
}
