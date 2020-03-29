using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
    enum MessageStatus
    {
        Sending,
        Error,
        Sent
    }


    class MessageViewModel : ViewModelBase
    {
        private readonly IChatApiHandler handler;
        private readonly Guid roomGuid;

        public MessageViewModel(IChatApiHandler handler, Guid roomGuid)
        {
            this.handler = handler;
            this.roomGuid = roomGuid;
        }

        public async Task Send()
        {
            if (Status == MessageStatus.Sent)
                return;

            Status = MessageStatus.Sending;

            if (Attachments.Any(a => a.FileModel == null))
            {
                return;
            }

            if (Attachments.Count == 0)
                await handler.SendToRoom(Text, roomGuid);
            else
                await handler.SendToRoomWithAttachments(Text, roomGuid, Attachments.Select(a => a.FileModel.Id).ToList());
        }

        #region Properties

        private string text;

        public string Text
        {
            get { return text; }
            set => Set(ref text, nameof(Text), value);
        }


        private ObservableCollection<AttachmentViewModel> attachments;

        public ObservableCollection<AttachmentViewModel> Attachments
        {
            get { return attachments; }
            set => Set(ref attachments, nameof(Attachments), value);
        }


        private MessageStatus status = MessageStatus.Sending;

        public MessageStatus Status
        {
            get { return status; }
            set => Set(ref status, nameof(Status), value);
        }

        #endregion

        #region Commands

        private ICommand _sendCommand;

        public ICommand SendCommand => _sendCommand ?? (_sendCommand = new AsyncRelayCommand(Send));

        #endregion
    }
}