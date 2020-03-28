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

        public Guid RefreshToken { get; set; }
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
            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostJson(
                    configuration.AppendPathToMainUrl(ApiReference.AUTHENTICATION_ROUTE),
                    request
                    );
            }
            catch(HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();

            var data = await response.GetJson<AuthenticationResponse>();

             return new Api(
                 new SessionInfo
                 {
                     Token = data.AccessToken,
                     RefreshToken = data.RefreshToken,
                     ExpiresAt = DateTime.MaxValue, // TODO
                 }
             , configuration);
        }

        public async Task<IApi> CreateApi(SessionInfo session)
        {
            var api = new Api(session, configuration);
            await api.GetUserAsync();

            return api;
        }

        public async Task<bool> Ping()
        {
            try
            {
                var response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(ApiReference.PING_ROUTE));
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IApi> RegisterUserAndCreateApi(RegistrationRequest request)
        {
            // PostJson -> Extensions.cs
            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostJson(
                    configuration.AppendPathToMainUrl(ApiReference.REGISTRATION_ROUTE),
                    request
                    );
            }
            catch (HttpRequestException exc)
            {
                throw new ServiceUnavailableException(exc.Message);
            }

            await response.ThrowErrorIfNotSuccessful();

            var data = await response.GetJson<RegistrationResponse>();

            return await CreateApi(new AuthenticationRequest { Login = data.User.UserName, Password = request.Password });
        }
    }
}
