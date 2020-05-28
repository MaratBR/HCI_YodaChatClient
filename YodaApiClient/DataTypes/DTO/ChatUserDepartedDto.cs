using System;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatUserDepartedDto
    {
        public Guid RoomId { get; set; }

        public int UserId { get; set; }
    }
}