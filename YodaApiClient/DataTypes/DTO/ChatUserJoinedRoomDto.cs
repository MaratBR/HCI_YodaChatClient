using System;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatUserJoinedRoomDto
    {
        public ChatUserDto User { get; set; }

        public Guid RoomId { get; set; }
    }
}