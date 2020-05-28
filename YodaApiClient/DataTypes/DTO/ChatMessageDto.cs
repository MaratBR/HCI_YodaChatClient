using System;
using System.Collections.Generic;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatMessageDto
    {
        public long Id { get; set; }

        public string Text { get; set; }

        public List<ChatAttachmentDto> Attachments { get; set; }

        public ChatMessageSenderDto Sender { get; set; }

        public Guid RoomId { get; set; }

        public DateTime PublishedAt { get; set; }
    }
}