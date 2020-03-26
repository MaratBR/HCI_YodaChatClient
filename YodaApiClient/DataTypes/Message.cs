using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes
{
    public class Message
    {
        public long Id { get; set; }

        public Guid SenderId { get; set; }

        public Guid RoomId { get; set; }

        public string Text { get; set; }

        public IEnumerable<Guid> Attachments { get; set; }

        public DateTime PublishedAt { get; set; }
    }
}
