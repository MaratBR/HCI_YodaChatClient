using System;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatAttachmentDto
    {
        public Guid Id { get; set; }

        public int Size { get; set; }

        public string Name { get; set; }
    }
}