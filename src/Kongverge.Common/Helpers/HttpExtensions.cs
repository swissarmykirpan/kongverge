using System.Net.Http;
using System.Text;

namespace Kongverge.Common.Helpers
{
    public static class HttpExtensions
    {
        public static StringContent AsJsonStringContent(this string json) =>
            new StringContent(json, Encoding.UTF8, "application/json");
    }
}
