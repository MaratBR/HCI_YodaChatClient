using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient
{
    public struct MessageQueueStatus
    {
        public bool Sent;

        public string Error;

        public long? Id;
    }

    public interface IMessageQueue
    {
        Task<MessageQueueStatus> PutToQueue(IMessageHandler message);
    }
}
