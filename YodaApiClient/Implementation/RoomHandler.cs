using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.DTO;
using YodaApiClient.Events;

namespace YodaApiClient.Implementation
{
    class RoomHandler : IRoomHandler
    {
        private readonly ChatClient client;
        private readonly Dictionary<Guid, IUser> usersCache = new Dictionary<Guid, IUser>();
        private Room room;

        public event ChatEventHandler<ChatMessageDto> MessageReceived;
        public event ChatEventHandler<UserJoinedRoomDto> UserJoined;
        public event ChatEventHandler<UserDepartedDto> UserDeparted;

        public Guid Id => room?.Id ?? Guid.Empty;

        public string Name => room?.Name;

        public string Description => room?.Description;

        public IChatClient Client => client;

        public RoomHandler(Room room, ChatClient client)
        {
            this.room = room;
            this.client = client;

            // TODO optimize (!!!)
            this.client.MessageReceived += Handler_MessageReceived;
            this.client.UserLeft += Handler_UserLeft;
            this.client.UserJoined += Handler_UserJoined;
        }

        ~RoomHandler()
        {

        }

        #region Event handlers
        private void Handler_UserLeft(object sender, ChatEventArgs<UserDepartedDto> e)
        {
            if (e.InnerMessage.RoomId == Id)
            {
                UserDeparted?.Invoke(this, e);
            }
        }

        private void Handler_UserJoined(object sender, ChatEventArgs<UserJoinedRoomDto> e)
        {
            if (e.InnerMessage.RoomId == Id)
            {
                UserJoined?.Invoke(this, e);
            }
        }

        private void Handler_MessageReceived(object sender, ChatEventArgs<ChatMessageDto> e)
        {
            if (e.InnerMessage.RoomId == Id)
            {
                MessageReceived?.Invoke(this, e);
            }
        }

        #endregion

        public Task<MessageQueueStatus> PutToQueue(ChatMessageRequestDto message)
        {
            return client.PutToQueue(message);
        }
    }
}
