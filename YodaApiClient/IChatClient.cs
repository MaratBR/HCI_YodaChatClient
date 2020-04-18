

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.DTO;
using YodaApiClient.Events;

namespace YodaApiClient
{
    public interface IChatClient
    {
        IApi Api { get; }

        User User { get; }

        [Obsolete] Task SendToRoom(string text, Guid roomId);

        [Obsolete] Task SendToRoomWithAttachments(string text, Guid roomId, IList<Guid> fileGuids);

        Task JoinRoomAsync(Guid roomId);

        Task LeaveRoomAsync(Guid roomId);

        event ChatEventHandler<ChatMessageDto> MessageReceived;

        [Obsolete]
        event ChatEventHandler<ChatUserJoinedRoomDto> UserJoined;

        [Obsolete]
        event ChatEventHandler<ChatUserDepartedDto> UserLeft;

        event ChatEventHandler<UserStatusDto> UserStatusChanged;

        Task<IRoomHandler> GetRoomHandlerAsync(Guid id);

        Task Connect();

        Task Disconnect();
    }

    public static class ChatApiHandlerExtension
    {
        public static Task<User> GetUserAsync(this IChatClient handler) => handler.Api.GetUserAsync();
    }
}