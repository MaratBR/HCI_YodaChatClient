using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
