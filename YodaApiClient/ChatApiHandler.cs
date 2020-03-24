using Microsoft.AspNetCore.SignalR.Client;
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


        public ChatApiHandler(string accessToken, ApiConfiguration configuration)
        {
            this.accessToken = accessToken;
            this.configuration = configuration;
        }

        public async Task Connect()
        {
            connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(configuration.AppendPathToMainUrl(ApiReference.SIGNALR_HUB_ROUTE), options => options.Headers.Add("Authorization", $"Bearer {accessToken}"))
                .Build();
            await connection.StartAsync();

            InitHubProxy();
        }

        private void InitHubProxy()
        {
            connection.On<int, int>("Left", YODAHub_Left);
            connection.On<int, int>("Joined", YODAHub_Joined);
            connection.On<string, int, int>("Text", YODAHub_Text);
        }

        #region Handling SignalR events

        private void YODAHub_Joined(int userId, int roomId)
        {
            UserJoined?.Invoke(this, new ChatUserJoinedEventArgs { RoomId = roomId, UserId = userId });
        }

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

        public Task JoinRoom(int roomId) => connection.InvokeAsync("JoinRoom", roomId);

        public Task LeaveRoom(int roomId) => connection.InvokeAsync("LeaveRoom", roomId);

        public Task SendToRoom(string text, int roomId) => connection.InvokeAsync("Send", text, roomId);

        #endregion
    }
}