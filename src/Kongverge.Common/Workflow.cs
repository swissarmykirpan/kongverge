using System;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kongverge.Common
{
    public abstract class Workflow
    {
        protected Workflow(IKongAdminReader kongReader, IOptions<Settings> configuration)
        {
            KongReader = kongReader;
            Configuration = configuration.Value;
        }

        protected IKongAdminReader KongReader { get; }
        protected Settings Configuration { get; }

        public async Task<int> Execute()
        {
            Log.Information("Getting existing services from {host}\n", Configuration.Admin.Host);

            var reachable = await KongReader.KongIsReachable().ConfigureAwait(false);
            if (!reachable)
            {
                return ExitWithCode.Return(ExitCodes.HostUnreachable);
            }

            try
            {
                return await DoExecute().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while process job");
                throw;
            }
        }

        public abstract Task<int> DoExecute();
    }
}
