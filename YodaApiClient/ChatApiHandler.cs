using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;
using YodaApiClient.Constants;

namespace YodaApiClient
{
    internal class ChatApiHandler : IChatApiHandler
    {
        private string accessToken;
        private ApiConfiguration configuration;
        private HubConnection connection;
        private IHubProxy hubProxy;


        public ChatApiHandler(string accessToken, ApiConfiguration configuration)
        {
            this.accessToken = accessToken;
            this.configuration = configuration;
        }

        public async Task Connect()
        {
            connection = new HubConnection(configuration.AppendPathToMainUrl(ApiReference.SIGNALR_HUB_ROUTE));
            connection.Headers.Add("Authorization", $"Bearer {accessToken}");
            await connection.Start();

            InitHubProxy();
        }

        private void InitHubProxy()
        {
            hubProxy = connection.CreateHubProxy("YODAHub");
            hubProxy.On<int, int>("Left", YODAHub_Left);
            hubProxy.On<string, int, int>("Text", YODAHub_Text);
        }



        #region Handling SignalR events

        private void YODAHub_Left(int userId, int roomId)
        {
            UserLeft?.Invoke(this, new ChatUserLeftEventArgs { UserId = userId, RoomId = roomId });
        }

        private void YODAHub_Text(string text, int roomId, int senderId)
        {
            MessageReceived?.Invoke(this, new ChatMessageEventArgs { SenderId = senderId, RoomId = roomId, Text = text });
        }

        #endregion

        #region Events

        public event EventHandler<ChatMessageEventArgs> MessageReceived;
        public event EventHandler<ChatUserJoinedEventArgs> UserJoined;
        public event EventHandler<ChatUserLeftEventArgs> UserLeft;

        #endregion

        #region Implementation

        public Task JoinRoom(int roomId)
        {
            connection.Se
        }

        public Task LeaveRoom(int roomId)
        {
            throw new NotImplementedException();
        }

        public Task SendToRoom(string text, int roomId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}