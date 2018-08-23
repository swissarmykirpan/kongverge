using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kongverge.Common.Workflow
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
            var existingServices = await KongReader.GetServices().ConfigureAwait(false);
            var existingGlobalConfig = await KongReader.GetGlobalConfig().ConfigureAwait(false);

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

            var processor = new KongProcessor(_kongWriter, _kongPluginCollection);

            await processor.Process(existingServices, servicesFromFile, existingGlobalConfig, newGlobalConfig)
                .ConfigureAwait(false);

            return ExitWithCode.Return(ExitCode.Success);
        }
    }
}
