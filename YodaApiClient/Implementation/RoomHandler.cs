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

        public Guid Id => room?.Id ?? Guid.Empty;

        public string Name => room?.Name;

        public string Description => room?.Description;

        public IApi API => chatApiHandler.Api;

        public RoomHandler(Room room, ChatApiHandler chatApiHandler)
        {
            this.room = room;
            this.chatApiHandler = chatApiHandler;

            // TODO optimize (!!!)
            chatApiHandler.MessageReceived += Handler_MessageReceived;
            chatApiHandler.UserActionPerformed += ChatApiHandler_UserActionPerformed;
        }

        ~RoomHandler()
        {
            chatApiHandler.MessageReceived -= Handler_MessageReceived;
        }

        #region Event handlers

        private void ChatApiHandler_UserActionPerformed(object sender, ChatUserActionEventArgs e)
        {
            UserActionPerformed?.Invoke(this, e);
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

        #endregion

        public event EventHandler<ChatMessageEventArgs> MessageReceived;
        public event EventHandler<ChatUserJoinedEventArgs> UserJoined;
        public event EventHandler<ChatUserLeftEventArgs> UserLeft;
        public event EventHandler<ChatUserActionEventArgs> UserActionPerformed;

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
