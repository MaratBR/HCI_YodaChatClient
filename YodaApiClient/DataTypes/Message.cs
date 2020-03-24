using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes
{
    public class Message
    {
        public int SenderId { get; set; }

        public string Text { get; set; }

        public int? FileId { get; set; }
    }
}
