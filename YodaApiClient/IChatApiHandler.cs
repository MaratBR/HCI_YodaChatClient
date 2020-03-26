

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient
{
    public class ChatEventArgs : EventArgs
    {
    }

    public class ChatMessageEventArgs : ChatEventArgs
    {

        public Message Message { get; set; }
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

    public interface IChatApiHandler
    {
        Task SendToRoom(string text, Guid roomId);

        Task SendToRoomWithAttachments(string text, Guid roomId, IList<Guid> fileGuids);

        Task JoinRoom(Guid roomId);

        Task LeaveRoom(Guid roomId);

        event EventHandler<ChatMessageEventArgs> MessageReceived;

        event EventHandler<ChatUserJoinedEventArgs> UserJoined;

        event EventHandler<ChatUserLeftEventArgs> UserLeft;

    }
}