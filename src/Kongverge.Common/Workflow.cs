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
        protected Workflow(IKongAdminReadService adminReadService, IOptions<Settings> configuration)
        {
            KongAdminReadService = adminReadService;
            Configuration = configuration.Value;
        }

        protected IKongAdminReadService KongAdminReadService { get; }
        protected Settings Configuration { get; }

        public async Task<int> Execute()
        {
            Log.Information("Getting existing services from {host}\n", Configuration.Admin.Host);

            var reachable = await KongAdminReadService.KongIsReachable().ConfigureAwait(false);
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
