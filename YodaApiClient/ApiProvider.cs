using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.Constants;
using YodaApiClient.Helpers;

namespace YodaApiClient
{
    class AuthenticationResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    public class ApiProvider : IApiProvider
    {
        private readonly ApiConfiguration configuration;

        public ApiProvider(ApiConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IApi> CreateApi(AuthenticationRequest request)
        {
            var httpClient = new HttpClient();

            // PostJson -> Extensions.cs
            var response = await httpClient.PostJson(
                configuration.AppendPathToMainUrl(ApiReference.AUTHENTICATION_ROUTE),
                request
                );

            if (response.IsSuccessStatusCode)
            {
                var data = await response.GetJson<AuthenticationResponse>();

                return new Api(data.AccessToken);
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new InvalidCredentialsException();
                else
                    throw new UnexpectedHttpStatusCodeException(response.StatusCode);
            }
        }

        public Task<IApi> RegisterUserAndCreateApi(RegistrationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
