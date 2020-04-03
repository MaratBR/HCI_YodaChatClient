﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient
{
    public interface IRoomHandler : IMessageQueue
    {
        [Obsolete]
        IMessageSender GetMessageSender();

        string Name { get; }

        string Description { get; }

        Guid Id { get; }

        IMessageHandler CreateMessage();

        event EventHandler<ChatMessageEventArgs> MessageReceived;

        event EventHandler<ChatUserJoinedEventArgs> UserJoined;

        event EventHandler<ChatUserLeftEventArgs> UserLeft;

        IApi API { get; }

    }
}