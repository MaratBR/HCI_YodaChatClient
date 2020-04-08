using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YodaApiClient.Constants;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.DTO;
using YodaApiClient.Events;

namespace YodaApiClient.Implementation
{
    internal class ChatClient : IChatClient, IMessageQueue
    {
        internal struct MessageQueueEntry
        {
            public ChatMessageRequestDto Message;
            public TaskCompletionSource<MessageQueueStatus> Promise;
        }

        private ApiConfiguration configuration;
        private HubConnection connection;
        private Thread messagesWorkerThread;
        private readonly IDictionary<Guid, IRoomHandler> savedRooms = new Dictionary<Guid, IRoomHandler>();

        private BlockingCollection<MessageQueueEntry> messageQueue = new BlockingCollection<MessageQueueEntry>();
        private Dictionary<Guid, MessageQueueEntry> awaitingAckMessages = new Dictionary<Guid, MessageQueueEntry>();

        public event ChatEventHandler<ChatMessageDto> MessageReceived;
        public event ChatEventHandler<UserJoinedRoomDto> UserJoined;
        public event ChatEventHandler<UserDepartedDto> UserLeft;

        public IApi Api { get; }

        public User User { get; set; }

        public ChatClient(IApi api, ApiConfiguration configuration)
        {
            this.Api = api;
            this.configuration = configuration;
            StartSenderWorker();
        }

        public async Task Connect()
        {
            if (connection != null)
                return;
            connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(configuration.AppendPathToMainUrl(ApiReference.SIGNALR_HUB_ROUTE), options => options.Headers.Add("Authorization", $"Bearer {Api.GetAccessToken()}"))
                .Build();
            await connection.StartAsync();

            await Init();
        }

        public async Task Disconnect()
        {
            if (connection == null)
                return;

            await connection.StopAsync();
        }

        private async Task Init()
        {
            SubscribeToAll(); // "Handling SignalR events" region
            await Update();
        }

        private async Task Update()
        {
            await LoadRooms();
            User = await Api.GetUserAsync();
        }

        private async Task LoadRooms()
        {
            List<Room> rooms = await Api.GetRoomsAsync();

            foreach(var room in rooms)
            {
                savedRooms[room.Id] = new RoomHandler(room, this);
            }
        }

        #region Background sender worker

        private void StartSenderWorker()
        {
            messagesWorkerThread = new Thread(SenderWorkerLoop);
            messagesWorkerThread.IsBackground = true;
            messagesWorkerThread.Start();
        }

        private async void SenderWorkerLoop()
        {
            foreach (var message in messageQueue.GetConsumingEnumerable())
            {
                await SendMessage(message);
                await Task.Delay(50);
            }
        }

        private async Task SendMessage(MessageQueueEntry entry)
        {
            entry.Message.Stamp = Guid.NewGuid();
            awaitingAckMessages[entry.Message.Stamp] = entry;
            await connection.SendAsync("Send", entry.Message);
        }

        #endregion

        #region Handling SignalR events

        private void SubscribeToAll()
        {
            connection.On<ChatMessageDto>("NewMessage", YODAHub_Message);
            connection.On<MessageAckDto>("MessageAck", YODAHub_MessageAck);
            connection.On<UserJoinedRoomDto>("UserJoined", YODAHub_UserJoined);
            connection.On<UserDepartedDto>("UserLeft", YODAHub_UserDeparted);
        }

        private void YODAHub_UserJoined(UserJoinedRoomDto message)
        {
            UserJoined?.Invoke(
                this,
                new ChatEventArgs<UserJoinedRoomDto>(new ChatEventContext(this), message)
                );
        }

        private void YODAHub_UserDeparted(UserDepartedDto msg)
        {
            UserLeft?.Invoke(
                this,
                new ChatEventArgs<UserDepartedDto>(new ChatEventContext(this), msg)
                );
        }

        private void YODAHub_MessageAck(MessageAckDto ack)
        {
            if (awaitingAckMessages.ContainsKey(ack.Stamp))
            {
                var result = new MessageQueueStatus
                {
                    Id = ack.Id,
                    Sent = true
                };
                awaitingAckMessages[ack.Stamp].Promise.SetResult(result);
                awaitingAckMessages.Remove(ack.Stamp);
            }
        }

        private void YODAHub_Message(ChatMessageDto message)
        {
            MessageReceived?.Invoke(
                this,
                new ChatEventArgs<ChatMessageDto>(new ChatEventContext(this), message)
                );
        }

        #endregion

        #region IChatApiHandler implementation

        public Task JoinRoomAsync(Guid roomId) => connection.InvokeAsync("JoinRoom", roomId);

        public Task LeaveRoomAsync(Guid roomId) => connection.InvokeAsync("LeaveRoom", roomId);

        [Obsolete]
        public Task SendToRoom(string text, Guid roomId) => connection.InvokeAsync("Send", text, roomId);

        [Obsolete]
        public Task SendToRoomWithAttachments(string text, Guid roomId, IList<Guid> fileGuids) => connection.InvokeAsync("SendWithAttachments", text, roomId, fileGuids);

        public async Task<IRoomHandler> GetRoomHandlerAsync(Guid id)
        {
            var room = await Api.GetRoomAsync(id);

            return new RoomHandler(room, this);
        }

        #endregion

        #region IMessageQueue implementation

        public Task<MessageQueueStatus> PutToQueue(ChatMessageRequestDto message)
        {
            var promise = new TaskCompletionSource<MessageQueueStatus>();
            messageQueue.Add(new MessageQueueEntry { Promise = promise, Message = message });
            return promise.Task;
        }

        #endregion
    }
}