using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatAttachmentDto
    {
        public Guid Id { get; set; }

        public int Size { get; set; }

        public string Name { get; set; }
    }
}
