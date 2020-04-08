using System;
using System.Threading.Tasks;
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

        event ChatEventHandler<UserJoinedRoomDto> UserJoined;

        event ChatEventHandler<UserDepartedDto> UserDeparted;


    }
}
