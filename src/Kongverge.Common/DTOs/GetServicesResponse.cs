using System.Collections.Generic;

namespace Kongverge.Common.DTOs
{
    public class GetServicesResponse
    {
        public string Next { get; set; }
        public List<KongService> Data {get; set; }
    }

    public class GetRoutesResponse
    {
        public string Next { get; set; }
        public List<KongRoute> Data { get; set; }
    }
}
