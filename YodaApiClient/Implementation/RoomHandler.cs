using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.Implementation
{
    class RoomHandler : IRoomHandler
    {
        private readonly ChatApiHandler _handler;

        public Guid Id { get; }

        public string Name => "NOT IMPLEMENTED";

        public string Description => "NOT IMPLEMENTED";

        public IApi API => _handler.API;

        internal RoomHandler(Guid id, ChatApiHandler handler)
        {
            Id = id;
            _handler = handler;

            // TODO optimize (!!!)
            _handler.MessageReceived += Handler_MessageReceived;
            _handler.UserJoined += Handler_UserJoined;
            _handler.UserLeft += Handler_UserLeft;
        }

        ~RoomHandler()
        {
            _handler.MessageReceived -= Handler_MessageReceived;
            _handler.UserJoined -= Handler_UserJoined;
            _handler.UserLeft -= Handler_UserLeft;
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
            return _handler.PutToQueue(messageHandler);

        }

        public IMessageHandler CreateMessage()
        {
            return new MessageHandler(this, _handler.GetUser());
        }
    }
}
