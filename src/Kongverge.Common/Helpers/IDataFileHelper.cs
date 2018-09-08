using System.Collections.Generic;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Helpers
{
    public interface IDataFileHelper
    {
        bool GetDataFiles(string dataPath, out IReadOnlyCollection<KongService> services, out ExtendibleKongObject globalConfig);
        void WriteConfigFiles(IEnumerable<KongService> services, ExtendibleKongObject globalConfig);
    }
}
