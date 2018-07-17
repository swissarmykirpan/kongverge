using System;
using System.Threading.Tasks;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;
using Microsoft.Extensions.Options;
using Kongverge.Common.DTOs;
using Serilog;

namespace Kongverge
{
    public abstract class Workflow
    {
        protected readonly IKongAdminReadService _adminService;
        protected readonly Settings _configuration;

        protected Workflow(IKongAdminReadService adminService, IOptions<Settings> configuration)
        {
            _adminService = adminService;
            _configuration = configuration.Value;
        }

        public async Task<int> Execute()
        {
            Log.Information("Getting existing services from {host}\n", _configuration.Admin.Host);

            var reachable = await _adminService.KongIsReachable().ConfigureAwait(false);
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
