using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;

namespace Kongverge.Common.Workflow
{
    public class ExportWorkflow : Workflow
    {
        private readonly IDataFileHelper _fileHelper;

        public ExportWorkflow(
            IKongAdminReader kongReader,
            IOptions<Settings> configuration,
            IDataFileHelper fileHelper) : base(kongReader, configuration)
        {
            _fileHelper = fileHelper;
        }

        public override async Task<int> DoExecute()
        {
            var services = await KongReader.GetServices().ConfigureAwait(false);
            var plugins = await KongReader.GetPlugins().ConfigureAwait(false);
            var globalConfig = new ExtendibleKongObject
            {
                Plugins = plugins.Where(x => x.IsGlobal()).ToArray()
            };

            //Write Output Files
            _fileHelper.WriteConfigFiles(services, globalConfig);
            return ExitWithCode.Return(ExitCode.Success);
        }
    }
}
