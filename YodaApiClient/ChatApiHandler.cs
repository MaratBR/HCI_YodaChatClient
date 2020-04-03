using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YodaApiClient.Constants;
using YodaApiClient.DataTypes;
using YodaApiClient.Implementation;

namespace YodaApiClient
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
        private BlockingCollection<MessageQueueEntry> messages = new BlockingCollection<MessageQueueEntry>();
        private Thread messagesWorkerThread;
        private readonly IDictionary<Guid, IRoomHandler> roomHandlers = new Dictionary<Guid, IRoomHandler>();

        public IApi API { get; }

        public ChatApiHandler(IApi api, ApiConfiguration configuration)
        {
            this.API = api;
            this.configuration = configuration;
            StartSenderWorker();
        }

        private void StartSenderWorker()
        {
            messagesWorkerThread = new Thread(SenderWorkerLoop);
            messagesWorkerThread.IsBackground = true;
            messagesWorkerThread.Start();
        }

        private async void SenderWorkerLoop()
        {
            foreach (var message in messages.GetConsumingEnumerable())
            {
                await SendMessage(message);
            }
        }

        private async Task SendMessage(MessageQueueEntry message)
        {
            if (message.Message.Attachments.Count == 0)
            {
                await SendToRoom(message.Message.Text, message.Message.Room.Id);
            }
            else
            {
                // TODO improve implementation
                var attachements = message.Message.Attachments.Select(a => a.Id).ToList();
                await SendToRoomWithAttachments(message.Message.Text, message.Message.Room.Id, attachements);
            }
        }

        public async Task Connect()
        {
            connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(configuration.AppendPathToMainUrl(ApiReference.SIGNALR_HUB_ROUTE), options => options.Headers.Add("Authorization", $"Bearer {API.GetAccessToken()}"))
                .Build();
            await connection.StartAsync();
            user = await API.GetUserAsync();

            Init();
        }

        private void Init()
        {
            connection.On<Guid, Guid>("Left", YODAHub_Left);
            connection.On<Guid, Guid>("Joined", YODAHub_Joined);
            connection.On<Message>("Message", YODAHub_Message);
        }

        #region Handling SignalR events

        private void YODAHub_Joined(Guid userId, Guid roomId)
        {
            UserJoined?.Invoke(this, new ChatUserJoinedEventArgs { RoomId = roomId, UserId = userId });
        }

        private void YODAHub_Left(Guid userId, Guid roomId)
        {
            UserLeft?.Invoke(this, new ChatUserLeftEventArgs { UserId = userId, RoomId = roomId });
        }

        private async void YODAHub_Message(Message message)
        {
            IUser user = await FindUser(message.SenderId);
            ICollection<IFile> attachments = GetAllAttachments(message.Attachments);
            IMessageHandler messageHandler = new MessageHandler(GetRoomHandler(message.RoomId), user, message.Text, attachments);
            MessageReceived?.Invoke(this, new ChatMessageEventArgs { Message = messageHandler });
        }

        private ICollection<IFile> GetAllAttachments(IEnumerable<Guid> attachments)
        {
            return attachments.Select(a => new FileImpl(a, API) as IFile).ToList();
        }

        private Task<IUser> FindUser(Guid senderId)
        {
            return API.GetUserAsync(senderId);
        }

        #endregion

        #region Events

        public event EventHandler<ChatMessageEventArgs> MessageReceived;
        public event EventHandler<ChatUserJoinedEventArgs> UserJoined;
        public event EventHandler<ChatUserLeftEventArgs> UserLeft;

        #endregion

        #region IChatApiHandler implementation

        public Task JoinRoom(Guid roomId) => connection.InvokeAsync("JoinRoom", roomId);

        public Task LeaveRoom(Guid roomId) => connection.InvokeAsync("LeaveRoom", roomId);

        public Task SendToRoom(string text, Guid roomId) => connection.InvokeAsync("Send", text, roomId);

        public Task SendToRoomWithAttachments(string text, Guid roomId, IList<Guid> fileGuids) => connection.InvokeAsync("SendWithAttachments", text, roomId, fileGuids);

        public IRoomHandler GetRoomHandler(Guid id)
        {
            if (!roomHandlers.ContainsKey(id))
            {
                roomHandlers[id] = new RoomHandler(id, this);
            }

            return roomHandlers[id];
        }

        public IUser GetUser()
        {
            return user;
        }

        #endregion

        #region IMessageQueue implementation

        public Task<MessageQueueStatus> PutToQueue(IMessageHandler message)
        {
            var promise = new TaskCompletionSource<MessageQueueStatus>();
            messages.Add(new MessageQueueEntry { Promise = promise, Message = message });
            return promise.Task;
        }

        #endregion
    }
}