using System;
using System.IO;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kongverge.Common.Workflow
{
    public class KongvergeWorkflow : Workflow
    {
        private readonly IKongAdminWriter _kongWriter;
        private readonly IDataFileHelper _fileHelper;

        public KongvergeWorkflow(
            IKongAdminReader kongReader,
            IOptions<Settings> configuration,
            IKongAdminWriter kongWriter,
            IDataFileHelper fileHelper) : base(kongReader, configuration)
        {
            _kongWriter = kongWriter;
            _fileHelper = fileHelper;
        }

        public override async Task<int> DoExecute()
        {
            Log.Information("Reading files from {inputFolder}", Configuration.InputFolder);

            KongvergeConfiguration targetConfiguration;
            try
            {
                targetConfiguration = await _fileHelper.ReadConfiguration(Configuration.InputFolder).ConfigureAwait(false);
            }
            catch (DirectoryNotFoundException ex)
            {
                return ExitWithCode.Return(ExitCode.InputFolderUnreachable, ex.Message);
            }
            catch (InvalidConfigurationFileException ex)
            {
                return ExitWithCode.Return(ExitCode.InvalidConfigurationFile, $"Invalid configuration file {ex.Path}{Environment.NewLine}{ex.Message}");
            }

            var existingConfiguration = await GetExistingConfiguration().ConfigureAwait(false);
            
            var processor = new KongProcessor(_kongWriter);
            await processor.Process(existingConfiguration, targetConfiguration).ConfigureAwait(false);

            return ExitWithCode.Return(ExitCode.Success);
        }
    }
}
