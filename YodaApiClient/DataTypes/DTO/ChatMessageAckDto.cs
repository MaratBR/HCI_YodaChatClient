using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes.DTO
{
    public class ChatMessageAckDto
    {
        public Guid Stamp { get; set; }

        public long Id { get; set; }
    }
}
