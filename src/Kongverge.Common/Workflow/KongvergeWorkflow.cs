using System.Collections.Generic;
using System.Linq;
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
            Log.Information("Reading files from {input}", Configuration.InputFolder);
            var success = _fileHelper.GetDataFiles(Configuration.InputFolder, out var targetServices, out var targetGlobalConfig);
            if (!success)
            {
                return ExitWithCode.Return(ExitCode.InputFolderUnreachable);
            }
            
            foreach (var targetService in targetServices)
            {
                var valid = await ServiceValidationHelper.Validate(targetService).ConfigureAwait(false);
                if (!valid)
                {
                    return ExitWithCode.Return(ExitCode.InvalidDataFile, $"Invalid Data File: {targetService.Name}{Settings.FileExtension}");
                }
            }

            var plugins = await KongReader.GetPlugins().ConfigureAwait(false);
            var existingServices = await GetExistingServices(plugins).ConfigureAwait(false);
            var existingGlobalConfig = GetExistingGlobalConfig(plugins);
            
            var processor = new KongProcessor(_kongWriter);

            await processor.Process(existingServices, targetServices, existingGlobalConfig, targetGlobalConfig).ConfigureAwait(false);

            return ExitWithCode.Return(ExitCode.Success);
        }

        private async Task<IReadOnlyCollection<KongService>> GetExistingServices(IReadOnlyCollection<KongPlugin> plugins)
        {
            var services = await KongReader.GetServices().ConfigureAwait(false);
            var routes = await KongReader.GetRoutes().ConfigureAwait(false);

            foreach (var existingService in services)
            {
                PopulateServiceTree(existingService, routes, plugins);
            }

            return services;
        }

        private void PopulateServiceTree(
            KongService service,
            IReadOnlyCollection<KongRoute> routes,
            IReadOnlyCollection<KongPlugin> plugins)
        {
            service.Plugins = plugins.Where(x => x.ServiceId == service.Id).ToArray();
            service.Routes = routes.Where(x => x.Service.Id == service.Id).ToArray();
            foreach (var serviceRoute in service.Routes)
            {
                serviceRoute.Plugins = plugins.Where(x => x.RouteId == serviceRoute.Id).ToArray();
            }
        }

        private static ExtendibleKongObject GetExistingGlobalConfig(IReadOnlyCollection<KongPlugin> plugins)
        {
            return new ExtendibleKongObject
            {
                Plugins = plugins.Where(x => x.IsGlobal()).ToArray()
            };
        }
    }
}
