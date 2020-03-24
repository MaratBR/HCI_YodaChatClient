

using System;
using System.Threading.Tasks;

namespace YodaApiClient
{
    public class ChatEventArgs : EventArgs
    {
    }

    public class ChatMessageEventArgs : ChatEventArgs
    {
        public string Text { get; set; }

        public Guid RoomId { get; set; }

        public Guid SenderId { get; set; }
    }

    public class ChatUserJoinedEventArgs : ChatEventArgs
    {
        public Guid UserId { get; set; }

        public Guid RoomId { get; set; }
    }

    public class ChatUserLeftEventArgs : ChatEventArgs
    {
        public Guid UserId { get; set; }

        public Guid? RoomId { get; set; }
    }

    public interface IChatApiHandler
    {
        Task SendToRoom(string text, Guid roomId);

        Task JoinRoom(Guid roomId);

        Task LeaveRoom(Guid roomId);

        event EventHandler<ChatMessageEventArgs> MessageReceived;

        event EventHandler<ChatUserJoinedEventArgs> UserJoined;

        event EventHandler<ChatUserLeftEventArgs> UserLeft;

    }
}