

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

        public int RoomId { get; set; }

        public int SenderId { get; set; }
    }

    public class ChatUserJoinedEventArgs : ChatEventArgs
    {
        public int UserId { get; set; }

        public int RoomId { get; set; }
    }

    public class ChatUserLeftEventArgs : ChatEventArgs
    {
        public int UserId { get; set; }

        public int? RoomId { get; set; }
    }

    public interface IChatApiHandler
    {
        Task SendToRoom(string text, int roomId);

        Task JoinRoom(int roomId);

        Task LeaveRoom(int roomId);

        event EventHandler<ChatMessageEventArgs> MessageReceived;

        event EventHandler<ChatUserJoinedEventArgs> UserJoined;

        event EventHandler<ChatUserLeftEventArgs> UserLeft;

    }
}