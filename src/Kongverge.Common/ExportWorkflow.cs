using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;

namespace Kongverge.Common
{
    public class ExportWorkflow : Workflow
    {
        private readonly IDataFileHelper _fileHelper;

        public ExportWorkflow(
            IKongAdminReadService adminReadService,
            IOptions<Settings> configuration,
            IDataFileHelper fileHelper) :
            base(adminReadService, configuration)
        {
            _fileHelper = fileHelper;
        }

        public override async Task<int> DoExecute()
        {
            var existingServices = await KongAdminReadService.GetServices().ConfigureAwait(false);

            //Write Output Files
            _fileHelper.WriteConfigFiles(existingServices);
            return ExitWithCode.Return(ExitCodes.Success);
        }
    }
}
