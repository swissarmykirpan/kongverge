using System.Collections.Generic;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Helpers
{
    public interface IDataFileHelper
    {
        KongDataFile ParseFile(string filename);
        bool GetDataFiles(string dataPath, out IReadOnlyCollection<KongDataFile> dataFiles, out GlobalConfig globalConfig);
        void WriteConfigFiles(IEnumerable<KongService> existingServices);
    }
}
