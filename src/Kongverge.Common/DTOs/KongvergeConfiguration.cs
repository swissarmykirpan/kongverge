using System;
using System.Collections.Generic;

namespace Kongverge.Common.DTOs
{
    public class KongvergeConfiguration
    {
        public IReadOnlyCollection<KongService> Services { get; set; } = Array.Empty<KongService>();
        public ExtendibleKongObject GlobalConfig { get; set; } = new ExtendibleKongObject();
    }
}