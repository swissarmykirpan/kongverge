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
        public static async Task<bool> HostIsReachable(KongDataFile data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://{data.Service.Host}:{data.Service.Port}");
                try
                {
                    var response = await client.GetAsync("/");
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Log.Error("Unable to contact host: {host}", data.Service.Host);
                        return false;
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException.GetType() == typeof(HttpRequestException))
                    {
                        Log.Error("Unable to contact host: {host}", data.Service.Host);
                        return false;
                    }

                    Log.Error(e, "Unknown error");
                    throw;
                }
            }
            return true;
        }

        public static bool RoutesAreValid(KongDataFile data)
        {
            return data.Service.Routes.All(x => x.Paths != null);

            //ToDo: Check if routes Clash
        }

        public static async Task<bool> Validate(KongDataFile data)
        {
            var reachable = data.Service.ValidateHost==false || await HostIsReachable(data);
            return reachable && RoutesAreValid(data);
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
