using System;
using System.Net;

namespace Kongverge.Common.Services
{
    public class KongException : Exception
    {
        public KongException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}