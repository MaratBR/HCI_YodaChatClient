using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.Events
{
    public class ChatEventContext
    {
        public IChatClient Client { get; }
        public IApi Api => Client.Api;

        internal ChatEventContext(IChatClient client)
        {
            Client = client;
        }
    }
}
