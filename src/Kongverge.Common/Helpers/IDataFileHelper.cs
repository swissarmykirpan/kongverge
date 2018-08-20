using System.Collections.Generic;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Helpers
{
    public interface IDataFileHelper
    {
        bool GetDataFiles(string dataPath, out IReadOnlyCollection<KongDataFile> dataFiles, out GlobalConfig globalConfig);
        void WriteConfigFiles(IEnumerable<KongService> existingServices);
    }
}
