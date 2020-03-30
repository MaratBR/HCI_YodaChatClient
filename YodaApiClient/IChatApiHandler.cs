

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient
{
    #region Event args

    public class ChatEventArgs : EventArgs
    {
    }

    public class ChatMessageEventArgs : ChatEventArgs
    {
        public IMessageHandler Message { get; set; }
    }

    public class ChatUserJoinedEventArgs : ChatEventArgs
    {
        public Guid UserId { get; set; }

        public Guid RoomId { get; set; }
    }

    public class ChatUserLeftEventArgs : ChatEventArgs
    {
        public Guid UserId { get; set; }

        public Guid RoomId { get; set; }
    }

    #endregion

    public interface IChatApiHandler
    {
        [Obsolete]
        Task SendToRoom(string text, Guid roomId);

        [Obsolete]
        Task SendToRoomWithAttachments(string text, Guid roomId, IList<Guid> fileGuids);

        Task JoinRoom(Guid roomId);

        Task LeaveRoom(Guid roomId);

        event EventHandler<ChatMessageEventArgs> MessageReceived;

        event EventHandler<ChatUserJoinedEventArgs> UserJoined;

        event EventHandler<ChatUserLeftEventArgs> UserLeft;

        IRoomHandler GetRoomHandler(Guid id);

        IUser GetUser();

        IApi API { get; }
    }
}