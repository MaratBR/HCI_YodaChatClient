using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private IApi api;
        private ApiConfiguration configuration;
        private HubConnection connection;
        private Queue<MessageQueueEntry> messages = new Queue<MessageQueueEntry>();
        private readonly IDictionary<Guid, IRoomHandler> roomHandlers = new Dictionary<Guid, IRoomHandler>();

        public ChatApiHandler(IApi api, ApiConfiguration configuration)
        {
            this.api = api;
            this.configuration = configuration;
        }

        public async Task Connect()
        {
            connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(configuration.AppendPathToMainUrl(ApiReference.SIGNALR_HUB_ROUTE), options => options.Headers.Add("Authorization", $"Bearer {api.GetAccessToken()}"))
                .Build();
            await connection.StartAsync();
            user = await api.GetUserAsync();

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
            ICollection<IAttachment> attachments = GetAllAttachments(message.Attachments);
            IMessageHandler messageHandler = new MessageHandler(GetRoomHandler(message.RoomId), user, message.Text, attachments);
            MessageReceived?.Invoke(this, new ChatMessageEventArgs { Message = messageHandler });
        }

        private ICollection<IAttachment> GetAllAttachments(IEnumerable<Guid> attachments)
        {
            return attachments.Select(a => new Attachment(a) as IAttachment).ToList();
        }

        private Task<IUser> FindUser(Guid senderId)
        {
            return api.GetUserAsync(senderId);
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
            messages.Enqueue(new MessageQueueEntry { Promise = promise, Message = message });
            return promise.Task;
        }

        #endregion
    }
}