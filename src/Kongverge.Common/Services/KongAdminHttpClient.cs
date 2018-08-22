using System.Net.Http;

namespace Kongverge.Common.Services
{
    public class KongAdminHttpClient : HttpClient
    {
        public KongAdminHttpClient() : base(new EnsureSuccessHandler()) { }

        public KongAdminHttpClient(HttpMessageHandler innerHandler) : base(new EnsureSuccessHandler(innerHandler)) { }
    }
}
