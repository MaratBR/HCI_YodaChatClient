using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.Implementation
{
    class Attachment : IAttachment
    {
        public Attachment(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; }
    }
}
