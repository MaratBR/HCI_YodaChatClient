using System;
using System.Collections.Generic;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatMessageRequestDto
    {
        public Guid RoomId { get; set; }

        public string Text { get; set; }

        public Guid Stamp { get; set; }

        public List<Guid> Attachments { get; set; }
    }
}