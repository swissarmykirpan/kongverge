using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;

namespace Kongverge
{
    public class ExportWorkflow : Workflow
    {
        private readonly IDataFileHelper _fileHelper;

        public ExportWorkflow(IKongAdminReadService adminService, IDataFileHelper fileHelper, IOptions<Settings> configuration) :
            base(adminService, configuration)
        {
            _fileHelper = fileHelper;
        }

        public override async Task<int> DoExecute()
        {
            var existingServices = await _adminService.GetServices().ConfigureAwait(false);

            //Write Output Files
            _fileHelper.WriteConfigFiles(existingServices);
            return ExitWithCode.Return(ExitCodes.Success);
        }
    }
}
