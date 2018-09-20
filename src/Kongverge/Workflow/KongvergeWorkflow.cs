using System;
using System.IO;
using System.Threading.Tasks;
using Kongverge.DTOs;
using Kongverge.Helpers;
using Kongverge.Services;
using Microsoft.Extensions.Options;

namespace Kongverge.Workflow
{
    public class KongvergeWorkflow : Workflow
    {
        private readonly IKongAdminWriter _kongWriter;
        private readonly ConfigFileReader _configReader;

        public KongvergeWorkflow(
            IKongAdminReader kongReader,
            IOptions<Settings> configuration,
            IKongAdminWriter kongWriter,
            ConfigFileReader configReader) : base(kongReader, configuration)
        {
            _kongWriter = kongWriter;
            _configReader = configReader;
        }

        public override async Task<int> DoExecute()
        {
            KongvergeConfiguration targetConfiguration;
            try
            {
                targetConfiguration = await _configReader.ReadConfiguration(Configuration.InputFolder).ConfigureAwait(false);
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
