using System;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatMessageAckDto
    {
        public Guid Stamp { get; set; }

        public long Id { get; set; }
    }
}