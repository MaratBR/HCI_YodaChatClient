using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException() { }
        public ApiException(string message) : base(message) { }
        public ApiException(string message, Exception inner) : base(message, inner) { }
        protected ApiException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    class InvalidCredentialsException : ApiException
    {
        public InvalidCredentialsException()
            : base("Invalid credentials given")
        {
        }
    }

    [Serializable]
    class UnexpectedHttpStatusCodeException : ApiException
    {
        public UnexpectedHttpStatusCodeException(HttpStatusCode code)
            : base($"Unexpected status code: {code}")
        {
        }
    }
}
