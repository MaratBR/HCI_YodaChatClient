using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.Implementation
{
    class MessageHandler : IMessageHandler
    {
        public MessageHandler(IRoomHandler room, IUser user)
        {
            Room = room;
            User = user;
        }

        public MessageHandler(IRoomHandler room, IUser user, string text, ICollection<IFile> attachments)
            : this(room, user)
        {
            Status = MessageStatus.Received;
            Text = text;
            Attachments = attachments;
        }

        #region Implementation

        public long? Id { get; set; }

        public string Text { get; set; }

        public string Error { get; set; }

        public ICollection<IFile> Attachments { get; set; } = new List<IFile>();

        public MessageStatus status = MessageStatus.Draft;

        public MessageStatus Status
        {
            get => status;
            set
            {
                status = value;
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IRoomHandler Room { get; }

        public IUser User { get;  }

        public event EventHandler StatusChanged;

        public async Task<bool> Send()
        {
            Status = MessageStatus.Sending;
            var result = await Room.PutToQueue(this);

            if (result.Sent)
            {
                Status = MessageStatus.Sent;
                Id = result.Id;
                return true;
            }
            else
            {
                Status = MessageStatus.Error;
                Error = result.Error ?? "UNKNOWN ERROR";
                return false;
            }
        }

        #endregion
    }
}
