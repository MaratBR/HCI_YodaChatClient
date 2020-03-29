using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.Implementation
{
    class RoomHandler : IRoomHandler, IMessageSender
    {
        private readonly Guid id;
        private readonly IChatApiHandler _handler;

        internal RoomHandler(Guid id, IChatApiHandler handler)
        {
            this.id = id;
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
            if (e.RoomId == id)
            {
                UserLeft?.Invoke(this, e);
            }
        }

        private void Handler_UserJoined(object sender, ChatUserJoinedEventArgs e)
        {
            if (e.RoomId == id)
            {
                UserJoined?.Invoke(this, e);
            }
        }

        private void Handler_MessageReceived(object sender, ChatMessageEventArgs e)
        {
            if (e.Message.RoomId == id)
            {
                MessageReceived?.Invoke(this, e);
            }
        }

        public event EventHandler<ChatMessageEventArgs> MessageReceived;
        public event EventHandler<ChatUserJoinedEventArgs> UserJoined;
        public event EventHandler<ChatUserLeftEventArgs> UserLeft;

        public IMessageSender GetMessageSender()
        {
            return this;
        }

        public Task Send(string text)
        {
            return _handler.SendToRoom(text, id);
        }

        public Task SendWithAttachments(string text, IList<Guid> attachments)
        {
            return _handler.SendToRoomWithAttachments(text, id, attachments);
        }
    }
}
