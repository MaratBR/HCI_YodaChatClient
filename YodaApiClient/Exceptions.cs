using System;
using System.Net;
using YodaApiClient.DataTypes;

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
    public class InvalidCredentialsException : ApiException
    {
        public InvalidCredentialsException()
            : base("Invalid credentials given")
        {
        }
    }

    [Serializable]
    public class UnexpectedHttpStatusCodeException : ApiException
    {
        public UnexpectedHttpStatusCodeException(HttpStatusCode code)
            : base($"Unexpected status code: {code}")
        {
        }
    }


    [Serializable]
    public class BadRequestException : ApiException
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

    [Serializable]
    public class ServiceUnavailableException : ApiException
    {
        public ServiceUnavailableException(string message)
            : base(message)
        {
        }
    }

}
