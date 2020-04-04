using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient.Implementation
{
    class RoomHandler : IRoomHandler
    {
        private readonly ChatApiHandler chatApiHandler;
        private Room room;

        public Guid Id { get; }

        public string Name => "NOT IMPLEMENTED";

        public string Description => "NOT IMPLEMENTED";

        public IApi API => chatApiHandler.API;

        [Obsolete]
        internal RoomHandler(Guid id, ChatApiHandler handler)
        {
            Id = id;
            chatApiHandler = handler;

            // TODO optimize (!!!)
            chatApiHandler.MessageReceived += Handler_MessageReceived;
            chatApiHandler.UserJoined += Handler_UserJoined;
            chatApiHandler.UserLeft += Handler_UserLeft;
        }

        public RoomHandler(Room room, ChatApiHandler chatApiHandler)
        {
            this.room = room;
            this.chatApiHandler = chatApiHandler;

            // TODO optimize (!!!)
            chatApiHandler.MessageReceived += Handler_MessageReceived;
            chatApiHandler.UserJoined += Handler_UserJoined;
            chatApiHandler.UserLeft += Handler_UserLeft;
        }

        ~RoomHandler()
        {
            chatApiHandler.MessageReceived -= Handler_MessageReceived;
            chatApiHandler.UserJoined -= Handler_UserJoined;
            chatApiHandler.UserLeft -= Handler_UserLeft;
        }

        private void Handler_UserLeft(object sender, ChatUserLeftEventArgs e)
        {
            if (e.RoomId == Id)
            {
                UserLeft?.Invoke(this, e);
            }
        }

        private void Handler_UserJoined(object sender, ChatUserJoinedEventArgs e)
        {
            if (e.RoomId == Id)
            {
                UserJoined?.Invoke(this, e);
            }
        }

        private void Handler_MessageReceived(object sender, ChatMessageEventArgs e)
        {
            if (e.Message.Room.Id == Id)
            {
                MessageReceived?.Invoke(this, e);
            }
        }

        public event EventHandler<ChatMessageEventArgs> MessageReceived;
        public event EventHandler<ChatUserJoinedEventArgs> UserJoined;
        public event EventHandler<ChatUserLeftEventArgs> UserLeft;

        public Task<MessageQueueStatus> PutToQueue(IMessageHandler messageHandler)
        {
            return chatApiHandler.PutToQueue(messageHandler);

        }

        public IMessageHandler CreateMessage()
        {
            return new MessageHandler(this, chatApiHandler.GetUser());
        }
    }
}
