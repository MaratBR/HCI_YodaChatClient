using System;

namespace YodaApiClient.Events
{
    public class ChatEventArgs<T> : EventArgs
    {
        public ChatEventContext Context { get; set; }

        public ChatEventArgs(ChatEventContext context, T inner)
        {
            Context = context;
            InnerMessage = inner;
        }

        public T InnerMessage { get; }
    }

    public delegate void ChatEventHandler<T>(object sender, ChatEventArgs<T> args);
}