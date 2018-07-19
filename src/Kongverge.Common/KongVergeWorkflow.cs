using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Kongverge.Extension;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kongverge
{
    public class KongvergeWorkflow : Workflow
    {
        new protected IKongAdminService _adminService;
        private readonly IDataFileHelper _fileHelper;
        private readonly IExtensionCollection _extensionCollection;

        public KongvergeWorkflow(IKongAdminService adminService, IDataFileHelper fileHelper, IOptions<Settings> configuration, IExtensionCollection extensionCollection)
            : base(adminService, configuration)
        {
            _fileHelper = fileHelper;
            _adminService = adminService;
            _extensionCollection = extensionCollection;
        }

        public override async Task<int> DoExecute()
        {
            var existingServices = await _adminService.GetServices().ConfigureAwait(false);

            Log.Information("Reading files from {input}", _configuration.InputFolder);
            var success = _fileHelper.GetDataFiles(_configuration.InputFolder, out var dataFiles);
            if (!success)
            {
                return ExitWithCode.Return(ExitCodes.InputFolderUnreachable);
            }

            //Process Input Files
            var processedFiles = await ProcessFiles(existingServices, dataFiles).ConfigureAwait(false);

            //Remove Missing Services
            var missingServices = existingServices
                .Except(processedFiles)
                .ToList();

            if (missingServices.Count == 0)
            {
                return ExitWithCode.Return(ExitCodes.Success);
            }

            await DeleteServicesMissingFromConfig(missingServices).ConfigureAwait(false);

            return ExitWithCode.Return(ExitCodes.Success);
        }

        private async Task DeleteServicesMissingFromConfig(List<KongService> missingServices)
        {
            Log.Information("\nDeleting old services:");
            foreach (var service in missingServices)
            {
                Log.Information("Deleting service \"{serviceName}\"", service.Name);

                await _adminService.DeleteRoutes(service).ConfigureAwait(false);
                await _adminService.DeleteService(service).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<KongService>> ProcessFiles(
            List<KongService> existingServices,
            List<KongDataFile> dataFiles)
        {
            var processedFiles = new List<KongService>();
            foreach (var data in dataFiles)
            {
                processedFiles.Add(data.Service);
                await ProcessFile(existingServices, data).ConfigureAwait(false);
            }

            return processedFiles;
        }

        public async Task ProcessFile(List<KongService> existingServices, KongDataFile data)
        {
            var existingService = existingServices.Find(x => x.Name == data.Service.Name);

            if (existingService == null)
            {
                Log.Information("\nAdding new service: \"{name}\"", data.Service.Name);

                var valid = await ServiceValidationHelper.Validate(data).ConfigureAwait(false);

                if (!valid)
                {
                    Log.Information("Invalid Data File: {name}{ext}", data.Service.Name, Settings.FileExtension);
                    return;
                }

                var serviceAdded = await _adminService.AddService(data.Service).ConfigureAwait(false);

                if (serviceAdded.Succeeded)
                {
                    await ConvergePlugins(serviceAdded.Result).ConfigureAwait(false);

                    await ConvergeRoutes(data.Service, data.Service.Routes).ConfigureAwait(false);
                }
            }
            else
            {
                // TODO: Clean this up, its messy, but where else can we do this?
                data.Service.Id = existingService.Id;

                await ConvergePlugins(data.Service, existingService).ConfigureAwait(false);

                await ConvergeRoutes(data.Service, existingService.Routes).ConfigureAwait(false);

                if (!ServiceHasChanged(existingService, data.Service))
                {
                    return;
                }

                Log.Information("Updating service: \"{name}\"", data.Service.Name);

                await _adminService.UpdateService(data.Service).ConfigureAwait(false);
            }
        }

        public Task ConvergePlugins(KongService result)
        {
            return ConvergePlugins(result, ExtendibleKongObject.Empty);
        }

        public async Task ConvergeRoutes(KongService service, IEnumerable<KongRoute> existingRoutes)
        {
            var toAdd = service.Routes.Except(existingRoutes);
            var toRemove = existingRoutes.Except(service.Routes);

            await Task.WhenAll(toRemove.Select(r => _adminService.DeleteRoute(r))).ConfigureAwait(false);
            await Task.WhenAll(toAdd.Select(r => _adminService.AddRoute(service, r))).ConfigureAwait(false);

            var matchingRoutePairs =
                service.Routes.Select(r => new ExtendibleKongObjectTargetPair(r, existingRoutes));

            foreach (var routepair in matchingRoutePairs)
            {
                // TODO: Clean up same as before - the targets when loaded from file don't have IDs?
                //routepair.Target.Id = routepair.Existing.Id;

                await ConvergePlugins(routepair.Target, routepair.Existing).ConfigureAwait(false);
            }
        }

        public async Task ConvergePlugins(ExtendibleKongObject target, ExtendibleKongObject existing)
        {
            var newSet = target.Extensions.SafeIfNull().ToDictionary(e => e.GetType()) ?? new Dictionary<Type, IKongPluginConfig>();
            var existingSet = existing?.Extensions.SafeIfNull().ToDictionary(t => t.GetType()) ?? new Dictionary<Type, IKongPluginConfig>();

            var changes = newSet.Keys.Union(existingSet.Keys)
                .Select(key =>
                    new
                    {
                        Target = newSet.ContainsKey(key) ? newSet[key] : null,
                        Existing = existingSet.ContainsKey(key) ? existingSet[key] : null
                    });

            foreach (var change in changes)
            {
                if (change.Target == null)
                {
                    await _adminService.DeletePlugin(change.Existing.id).ConfigureAwait(false);
                }
                else if (change.Existing == null)
                {
                    var content = _extensionCollection.CreatePluginBody(change.Target);

                    await _adminService.UpsertPlugin(target.DecoratePluginBody(content)).ConfigureAwait(false);
                }
                else if(!change.Target.IsExactMatch(change.Existing))
                {
                    var content = _extensionCollection.CreatePluginBody(change.Target);

                    content.id = change.Existing.id;

                    // TODO: Same problem here - target has come from a file, and it doesn't have the Created info to feed into created_at
                    target.Created = existing.Created;

                    await _adminService.UpsertPlugin(target.DecoratePluginBody(content)).ConfigureAwait(false);
                }
            }
        }

        public static bool ServiceHasChanged(KongService existingService, KongService newService)
        {
            if (!existingService.Equals(newService))
            {
                ServiceValidationHelper.PrintDiff(existingService, newService);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
