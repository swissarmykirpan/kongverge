using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;

namespace Kongverge.Common.Workflow
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
            var existingConfiguration = await GetExistingConfiguration().ConfigureAwait(false);

            await _configWriter.WriteConfiguration(existingConfiguration, Configuration.OutputFolder).ConfigureAwait(false);

            return ExitWithCode.Return(ExitCode.Success);
        }
    }
}
