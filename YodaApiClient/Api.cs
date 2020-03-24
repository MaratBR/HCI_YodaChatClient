using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YodaApiClient.Constants;
using YodaApiClient.DataTypes;
using YodaApiClient.Helpers;

namespace YodaApiClient
{
    class RoomsResponse
    {
        public ICollection<Room> Rooms { get; set; }
    }

    internal class Api : IApi
    {
        private string accessToken;
        private readonly ApiConfiguration configuration;
        private readonly HttpClient httpClient;

        private User user;
        private DateTime lastUserUpdate;

        public Api(string accessToken, ApiConfiguration configuration)
        {
            this.accessToken = accessToken;
            this.configuration = configuration;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }

        public async Task<IChatApiHandler> Connect()
        {
            var handler = new ChatApiHandler(accessToken, configuration);
            await handler.Connect();

            return handler;
        }

        public async Task<Room> CreateRoom(CreateRoomRequest request)
        {
            var response = await httpClient.PostJson(configuration.AppendPathToMainUrl(ApiReference.CREATE_ROOM_ROUTE), request);
            await response.ThrowErrorIfNotSuccessful();

            var room = await response.GetJson<Room>();

            return room;
        }

        public async Task<ICollection<Room>> GetRooms()
        {
            var response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(ApiReference.LIST_OF_ROOMS_ROUTE));
            await response.ThrowErrorIfNotSuccessful();

            var roomsResponse = await response.GetJson<RoomsResponse>();
            return roomsResponse.Rooms;
        }

        public async Task<User> GetUserAsync()
        {
            if (user == null || DateTime.Now - lastUserUpdate < TimeSpan.FromHours(1))
            {
                var response = await httpClient.GetAsync(configuration.AppendPathToMainUrl(ApiReference.CURRENT_USER_ROUTE));
                await response.ThrowErrorIfNotSuccessful();

                var user = await response.GetJson<User>();

                this.user = user;
            }

            return user;
        }
    }
}