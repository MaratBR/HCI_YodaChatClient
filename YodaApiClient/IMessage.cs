using System;
using System.Collections.Generic;
using System.IO;
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

        ICollection<IFile> Attachments { get; }

        IUser User { get; }

        MessageStatus Status { get; }

        IRoomHandler Room { get; }

        Task<bool> Send();

        event EventHandler StatusChanged;

    }

    public static class MessageHandlerExtension
    {
        public static bool CanBeSent(this IMessageHandler handler) => (handler.Status == MessageStatus.Draft || handler.Status == MessageStatus.Error) && !handler.IsEmpty();

        public static bool IsEmpty(this IMessageHandler handler) => string.IsNullOrWhiteSpace(handler.Text) && handler.Attachments.Count == 0;

        public static IFile AddAttachment(this IMessageHandler handler, Stream stream, long fileSize, string fileName)
        {
            IFile file = handler.Room.API.CreateFile(stream, fileSize, fileName);
            handler.Attachments.Add(file);
            return file;
        }
    }
}
