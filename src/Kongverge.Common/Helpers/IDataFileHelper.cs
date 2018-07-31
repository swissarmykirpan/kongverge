using System.Collections.Generic;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Helpers
{
    public interface IDataFileHelper
    {
        KongDataFile ParseFile(string filename);
        bool GetDataFiles(string dataPath, out List<KongDataFile> dataFiles, out GlobalConfig globalConfig);
        void WriteConfigFiles(List<KongService> existingServices);
    }
}
