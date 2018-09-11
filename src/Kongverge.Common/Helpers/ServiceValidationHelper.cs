using System.Collections.Generic;
using Kongverge.Common.DTOs;
using Serilog;

namespace Kongverge.Common.Helpers
{
    public static class ServiceValidationHelper
    {
        public static void PrintDiff(KongService existingService, KongService newService)
        {
            Log.Information("Changed Service: {name}", existingService.Name);
            var diff = existingService.DetailedCompare(newService);
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
