using FluentResults;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.Constants;

namespace YodaApp.YODApi
{
    public class A

    public class Api2
    {
        private readonly ApiConfiguration configuration = new ApiConfiguration();
        private readonly HttpClient client;
        private readonly ClientWebSocket wsClient;

        public static Api instance;

        public static Api Instance => instance ?? (instance = new Api());
        public static ApiConfiguration Configuration => Instance.configuration;

        private Api()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            wsClient = new ClientWebSocket();
        }

        #region Registration

        public class RegistrationRequest
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public string Email { get; set; }

            public string PhoneNumber { get; set; }

            public Gender? Gender { get; set; } = null;
        }

        public class RegistrationResponse
        {
            public User User { get; set; }
        }

        public async Task<Result<RegistrationResponse>> Register(RegistrationRequest request)
            => await Post<RegistrationResponse>(ApiReference.REGISTRATION_ROUTE, System.Net.HttpStatusCode.Created, request);

        #endregion

        #region Authentication

        public void SetToken(string access)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access}");
        }

        public class AuthenticateResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }

        public async Task<Result<AuthenticateResponse>> Authenticate(string login, string password)
            => await Post<AuthenticateResponse>("authentication/authenticate", System.Net.HttpStatusCode.OK, new { login = login, password = password });

        public async Task<User> GetCurrentUser()
        {
            var result = await Get<User>("authentication/user", System.Net.HttpStatusCode.OK);
            return result.ValueOrDefault;
        }

        #endregion

        #region Rooms

        public class RoomsResponse
        {
            public ICollection<Room> Rooms { get; set; }
        }


        public Task<Result<RoomsResponse>> GetRooms()
            => Get<RoomsResponse>("rooms", System.Net.HttpStatusCode.OK);

        public class CreateRoomRequest
        {
            public string Name { get; set; }

            public string Description { get; set; }
        }

        public Task<Result<Room>> CreateRoom(CreateRoomRequest request)
            => Post<Room>("rooms", System.Net.HttpStatusCode.Created, request);

        #endregion

        #region Chat

        #endregion

        #region Utility methods

        private string Uri(string path)
        {
            if (!path.StartsWith("/"))
                path = "/" + path;

            var proto = configuration.UseHTTPS ? "https" : "http";
            var uri = $"{proto}://{configuration.Domain}{configuration.BaseUrl}{path}";

            return uri;
        }

        private async Task<Result<T>> Get<T>(string path, System.Net.HttpStatusCode expectedStatus)
        {
            HttpResponseMessage response;

            try
            {
                response = await client.GetAsync(Uri(path));
            }
            catch (Exception e)
            {
                return Results.Fail<T>(e.ToString());
            }

            if (response.StatusCode == expectedStatus)
            {
                var text = await response.Content.ReadAsStringAsync();

                T result;

                try
                {
                    result = Json.Deserialize<T>(text);
                }
                catch (JsonException exc)
                {
                    return Results.Fail<T>(exc.ToString());
                }

                return Results.Ok<T>(result);
            }

            return Results.Fail<T>($"Unexpected status code, expected: {expectedStatus}, got: {response.StatusCode}");
        }

        private async Task<Result<T>> Post<T>(string path, System.Net.HttpStatusCode expectedStatus, object data)
        {
            HttpResponseMessage response;

            try
            {
                response = await client.PostAsync(
                    Uri(path),
                    new StringContent(
                        Json.Serialize(data),
                        Encoding.UTF8,
                        "application/json"
                        )
                    );
            }
            catch (Exception e)
            {
                return Results.Fail<T>(e.ToString());
            }

            var text = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == expectedStatus)
            {

                T result;

                try
                {
                    result = Json.Deserialize<T>(text);
                }
                catch (JsonException exc)
                {
                    return Results.Fail<T>(exc.ToString());
                }

                return Results.Ok<T>(result);
            }

            var error = new Error($"Unexpected status code, expected: {expectedStatus}, got: {response.StatusCode}");


            try
            {
                var value = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
                string message;

                if (value.ContainsKey("description"))
                {
                    message = value["description"].ToString();

                    if (value.ContainsKey("details"))
                    {
                        message += "\n" + value["details"].ToString();
                    }
                }
                else if (value.ContainsKey("title"))
                {
                    message = value["title"].ToString();
                }
                else
                {
                    message = $"Unexpected status code, expected: {expectedStatus}, got: {response.StatusCode}";
                }

                error = new Error(message);
            }
            catch(JsonException)
            {
                // nothing
            }

            return Results.Fail<T>(error);
        }

        #endregion

    }
}
