using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient
{
    public enum MessageStatus
    {
        Draft,
        Sending,
        Sent,
        Received,
        Error
    };

    public interface IMessageHandler
    {
        long? Id { get; }

        string Text { set; get; }

        string Error { get; }

        ICollection<IAttachment> Attachments { get; }

        IUser User { get; }

        MessageStatus Status { get; }

        IRoomHandler Room { get; }

        Task<bool> Send();
    }
}
