using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YodaApiClient.Constants;
using YodaApiClient.DataTypes;

namespace YodaApiClient.Implementation
{
    internal class ChatApiHandler : IChatApiHandler, IMessageQueue
    {
        internal struct MessageQueueEntry
        {
            public IMessageHandler Message;
            public TaskCompletionSource<MessageQueueStatus> Promise;
        }

        private IUser user;
        private ApiConfiguration configuration;
        private HubConnection connection;
        private Thread messagesWorkerThread;
        private readonly IDictionary<Guid, IRoomHandler> savedRooms = new Dictionary<Guid, IRoomHandler>();

        private BlockingCollection<MessageQueueEntry> messageQueue = new BlockingCollection<MessageQueueEntry>();
        private Dictionary<Guid, MessageQueueEntry> awaitingAckMessages = new Dictionary<Guid, MessageQueueEntry>();

        public IApi Api { get; }

        public ChatApiHandler(IApi api, ApiConfiguration configuration)
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
            user = await Api.GetUserAsync();

            await Init();
        }

        private async Task Init()
        {
            SubscribeToAll(); // "Handling SignalR events" region
            await Update();
        }

        private async Task Update()
        {
            await LoadRooms();
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
            }
        }

        private async Task SendMessage(MessageQueueEntry entry)
        {
            IMessageHandler message = entry.Message;
            var stamp = Guid.NewGuid();
            var dto = new ChatMessageRequestDto
            {
                Text = message.Text,
                Attachments = message.Attachments.Select(a => a.Id).ToList(),
                RoomId = message.Room.Id,
                Stamp = stamp
            };
            awaitingAckMessages[stamp] = entry;
            await connection.SendAsync("Send", dto);
        }

        #endregion

        #region Handling SignalR events

        private void SubscribeToAll()
        {
            connection.On<UserActionDto>("UserAction", YODAHub_UserAction);
            connection.On<ChatMessageDto>("NewMessage", YODAHub_Message);
            connection.On<MessageAckDto>("MessageAck", YODAHub_MessageAck);
        }

        private void YODAHub_UserAction(UserActionDto action)
        {
            UserActionPerformed?.Invoke(this, new ChatUserActionEventArgs { ActionDto = action });
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

        private async void YODAHub_Message(ChatMessageDto message)
        {
            IUser user = await FindUser(message.SenderId);
            ICollection<IFile> attachments = GetAllAttachments(message.Attachments);
            IRoomHandler roomHandler = await GetRoomHandlerAsync(message.RoomId);
            IMessageHandler messageHandler = new MessageHandler(roomHandler, user, message.Text, attachments);
            MessageReceived?.Invoke(this, new ChatMessageEventArgs { Message = messageHandler });
        }

        private ICollection<IFile> GetAllAttachments(IEnumerable<Guid> attachments)
        {
            return attachments.Select(a => new FileImpl(a, Api) as IFile).ToList();
        }

        private Task<IUser> FindUser(Guid senderId)
        {
            return Api.GetUserAsync(senderId);
        }

        #endregion

        #region Events

        public event EventHandler<ChatMessageEventArgs> MessageReceived;
        public event EventHandler<ChatUserActionEventArgs> UserActionPerformed;

        #endregion

        #region IChatApiHandler implementation

        public Task JoinRoomAsync(Guid roomId) => connection.InvokeAsync("JoinRoom", roomId);

        public Task LeaveRoomAsync(Guid roomId) => connection.InvokeAsync("LeaveRoom", roomId);

        [Obsolete]
        public Task SendToRoom(string text, Guid roomId) => connection.InvokeAsync("Send", text, roomId);

        [Obsolete]
        public Task SendToRoomWithAttachments(string text, Guid roomId, IList<Guid> fileGuids) => connection.InvokeAsync("SendWithAttachments", text, roomId, fileGuids);

        [Obsolete]
        public IRoomHandler GetRoomHandler(Guid id)
        {
            if (!savedRooms.ContainsKey(id))
            {
                return savedRooms[id];
            }

            throw new KeyNotFoundException($"Room with id {id} not found");
        }

        [Obsolete]
        public IUser GetUser()
        {
            return user;
        }

        public async Task<IRoomHandler> GetRoomHandlerAsync(Guid id)
        {
            var room = await Api.GetRoomAsync(id);

            return new RoomHandler(room, this);
        }

        #endregion

        #region IMessageQueue implementation

        public Task<MessageQueueStatus> PutToQueue(IMessageHandler message)
        {
            var promise = new TaskCompletionSource<MessageQueueStatus>();
            messageQueue.Add(new MessageQueueEntry { Promise = promise, Message = message });
            return promise.Task;
        }

        #endregion
    }
}