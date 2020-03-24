using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;
using YodaApiClient.Helpers;

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


    [Serializable]
    class BadRequestException : ApiException
    {
        public YODAError Error { get; private set; }

        public BadRequestException(YODAError error)
            : base(error.Description)
        {
            Error = error;
        }

        public BadRequestException()
            : base("Something went wrong while handling your request: request is invalid")
        {
        }
    }

}
