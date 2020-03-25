using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.Constants;
using YodaApiClient.DataTypes;
using YodaApiClient.Helpers;

namespace YodaApiClient
{
    class AuthenticationResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    class RegistrationResponse
    {
        public User User { get; set; }
    }

    /// <summary>
    /// Provides authentication and registration
    /// </summary>
    public class ApiProvider : IApiProvider
    {
        private readonly ApiConfiguration configuration;
        private readonly HttpClient httpClient;

        public ApiProvider(ApiConfiguration configuration)
        {
            this.configuration = configuration;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }


        public async Task<IApi> CreateApi(AuthenticationRequest request)
        {
            var httpClient = new HttpClient();

            // PostJson -> Extensions.cs
            var response = await httpClient.PostJson(
                configuration.AppendPathToMainUrl(ApiReference.AUTHENTICATION_ROUTE),
                request
                );
            await response.ThrowErrorIfNotSuccessful();

            var data = await response.GetJson<AuthenticationResponse>();

             return new Api(data.AccessToken, configuration);
        }

        public async Task<IApi> CreateApi(SessionInfo session)
        {
            return new Api(session.Token, configuration);
        }

        public async Task<IApi> RegisterUserAndCreateApi(RegistrationRequest request)
        {
            // PostJson -> Extensions.cs
            var response = await httpClient.PostJson(
                configuration.AppendPathToMainUrl(ApiReference.REGISTRATION_ROUTE),
                request
                );

            await response.ThrowErrorIfNotSuccessful();

            var data = await response.GetJson<RegistrationResponse>();

            return await CreateApi(new AuthenticationRequest { Login = data.User.UserName, Password = request.Password });
        }
    }
}
