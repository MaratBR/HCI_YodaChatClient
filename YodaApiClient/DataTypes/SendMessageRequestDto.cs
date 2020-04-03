using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes
{
    class SendMessageRequestDto
    {
        public Guid RoomId { get; set; }

        public string Text { get; set; }

        public Guid Stamp { get; set; }

        public List<Guid> Attachments { get; set; }
    }
}
