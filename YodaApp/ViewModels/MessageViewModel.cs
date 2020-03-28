using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient;

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
        private readonly IApi api;
        private readonly Guid roomGuid;

        public MessageViewModel(IApi api, Guid roomGuid)
        {
            this.api = api;
            this.roomGuid = roomGuid;
        }

        public async Task Send()
        {
            if (Status == MessageStatus.Sent)
                return;

            Status = MessageStatus.Sending;
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
    }
}