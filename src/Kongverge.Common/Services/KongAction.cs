using System;
using System.Net;

namespace Kongverge.Common.Services
{
    public class KongAction
    {
        public static KongAction<T> Success<T>(T result)
        {
            return Success(HttpStatusCode.OK, result);
        }

        public static KongAction<T> Success<T>(HttpStatusCode statusCode, T result)
        {
            return new KongAction<T>
            {
                Succeeded = true,
                StatusCode = HttpStatusCode.OK,
                ErrorMessage = string.Empty,
                Result = result
            };
        }


        public static KongAction<T> Failure<T>(HttpStatusCode statusCode, string errorMessage)
        {
            return new KongAction<T>
            {
                Succeeded = false,
                StatusCode = statusCode,
                ErrorMessage = errorMessage,
                Result = default(T)
            };
        }
    }

    public class KongAction<T>
    {
        public T Result { get; internal set; }

        public bool Succeeded { get; internal set;  }

        public string ErrorMessage { get; internal set; }

        public HttpStatusCode StatusCode { get; internal set; }

        public override string ToString()
        {
            if (Succeeded)
            {
                return $"Success with status {StatusCode} : {Result}";
            }

            return $"Failed with status {StatusCode} : {ErrorMessage}";
        }

        public void ShouldSucceed()
        {
            if (!Succeeded)
            {
                throw new Exception(ToString());
            }
        }
    }
}
