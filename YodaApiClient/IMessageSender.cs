using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient
{
    public interface IMessageSender
    {
        Task Send(string text);

        Task SendWithAttachments(string text, IList<Guid> attachments);
    }
}
