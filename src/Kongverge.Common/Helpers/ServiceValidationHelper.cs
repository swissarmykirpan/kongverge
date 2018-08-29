using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Serilog;

namespace Kongverge.Common.Helpers
{
    public static class ServiceValidationHelper
    {
        public static async Task<bool> HostIsReachable(KongService service)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://{service.Host}:{service.Port}");
                try
                {
                    var response = await client.GetAsync("/").ConfigureAwait(false);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Log.Error("Unable to contact host: {host}", service.Host);
                        return false;
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException.GetType() == typeof(HttpRequestException))
                    {
                        Log.Error("Unable to contact host: {host}", service.Host);
                        return false;
                    }

                    Log.Error(e, "Unknown error");
                    throw;
                }
            }
            return true;
        }

        public static bool RoutesAreValid(KongService service)
        {
            return service.Routes.All(x => x.Paths != null);

            //ToDo: Check if routes Clash
        }

        public static async Task<bool> Validate(KongService service)
        {
            var reachable = !service.ValidateHost || await HostIsReachable(service).ConfigureAwait(false);
            return reachable && RoutesAreValid(service);
        }

        public static void PrintDiff(KongService existingService, KongService newService)
        {
            Log.Information("Changed Service: {name}", existingService.Name);
            var diff = existingService.DetailedCompare(newService);
            PrintDiff(diff);
        }

        public static void PrintDiff(KongRoute existingRoute, KongRoute newRoute)
        {
            Log.Information("Changed Route: {paths}", existingRoute.Paths);
            var diff = existingRoute.DetailedCompare(newRoute);
            PrintDiff(diff);
        }

        public static void PrintDiff(IEnumerable<Variance> diff)
        {
            foreach (var variance in diff)
            {
                Log.Information($"\t{variance.Field}\t{variance.Existing}->{variance.New}");
            }
        }
    }
}
