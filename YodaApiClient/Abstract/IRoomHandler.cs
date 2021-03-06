﻿using System;
using YodaApiClient.DataTypes.DTO;
using YodaApiClient.Events;

namespace YodaApiClient
{
    public interface IRoomHandler : IMessageQueue
    {
        string Name { get; }

        string Description { get; }
        IChatClient Client { get; }

        Guid Id { get; }

        event ChatEventHandler<ChatMessageDto> MessageReceived;

        event ChatEventHandler<ChatUserJoinedRoomDto> UserJoined;

        event ChatEventHandler<ChatUserDepartedDto> UserDeparted;
    }
}