using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kongverge.Common
{
    public class KongvergeWorkflow : Workflow
    {
        private readonly IKongAdminWriter _kongWriter;
        private readonly IDataFileHelper _fileHelper;
        private readonly IKongPluginCollection _kongPluginCollection;

        public KongvergeWorkflow(
            IKongAdminReader kongReader,
            IOptions<Settings> configuration,
            IKongAdminWriter kongWriter,
            IDataFileHelper fileHelper,
            IKongPluginCollection kongPluginCollection)
            : base(kongReader, configuration)
        {
            _kongWriter = kongWriter;
            _fileHelper = fileHelper;
            _kongPluginCollection = kongPluginCollection;
        }

        public override async Task<int> DoExecute()
        {
            var existingGlobalConfig = await KongReader.GetGlobalConfig().ConfigureAwait(false);

            if (!existingGlobalConfig.Succeeded)
            {
                Log.Error("Unable to get current global config");
                return ExitWithCode.Return(ExitCode.HostUnreachable);
            }

            var existingServices = await KongReader.GetServices().ConfigureAwait(false);

            Log.Information("Reading files from {input}", Configuration.InputFolder);
            var success =
                _fileHelper.GetDataFiles(Configuration.InputFolder, out var dataFiles, out var newGlobalConfig);
            if (!success)
            {
                return ExitWithCode.Return(ExitCode.InputFolderUnreachable);
            }

            //Process Input Files
            var servicesFromFile = dataFiles
                .Select(f => f.Service)
                .ToList();

            var serviceWorkFlow = new ServicesWorkflow(_kongWriter, _kongPluginCollection);

            return await serviceWorkFlow.ProcessServices(existingServices, servicesFromFile,
                existingGlobalConfig.Result, newGlobalConfig);
        }
    }
}
