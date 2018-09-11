using System.Threading.Tasks;
using Kongverge.Common.DTOs;

namespace Kongverge.Common.Helpers
{
    public interface IDataFileHelper
    {
        Task<KongvergeConfiguration> ReadConfiguration(string folderPath);
        Task WriteConfiguration(KongvergeConfiguration configuration, string folderPath);
    }
}
