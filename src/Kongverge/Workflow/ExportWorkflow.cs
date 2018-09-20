using System.Threading.Tasks;
using Kongverge.DTOs;
using Kongverge.Helpers;
using Kongverge.Services;
using Microsoft.Extensions.Options;

namespace Kongverge.Workflow
{
    public class ExportWorkflow : Workflow
    {
        private readonly ConfigFileWriter _configWriter;

        public ExportWorkflow(
            IKongAdminReader kongReader,
            IOptions<Settings> configuration,
            ConfigFileWriter configWriter) : base(kongReader, configuration)
        {
            _configWriter = configWriter;
        }

        public override async Task<int> DoExecute()
        {
            var existingConfiguration = await GetExistingConfiguration();

            await _configWriter.WriteConfiguration(existingConfiguration, Configuration.OutputFolder);

            return ExitWithCode.Return(ExitCode.Success);
        }
    }
}
