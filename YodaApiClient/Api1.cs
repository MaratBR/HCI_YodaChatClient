using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient
{
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

        public Task<IChatApiHandler> Connect()
        {
            throw new System.NotImplementedException();
        }

        public Task<Room> CreateRoom(Room room)
        {
            throw new System.NotImplementedException();
        }

        public Task<ICollection<Room>> GetRooms()
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetUserAsync()
        {
            if (user != null && DateTime.Now - lastUserUpdate < TimeSpan.FromHours(1))
            {
                // update user
            }
        }
    }
}