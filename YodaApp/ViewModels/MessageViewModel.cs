using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.DTO;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
    enum MessageStatus
    {
        Sending,
        Sent,
        Error,
        Received
    }

    class MessageViewModel : ViewModelBase
    {
        private readonly IRoomHandler roomHandler;
        private ChatMessageDto message;
        private ChatMessageRequestDto request;

        public event EventHandler MessageSent;

        #region Properties

        private string text;

        public string Text
        {
            get { return text; }
            set => Set(ref text, nameof(Text), value);
        }

        private string sender;

        public string Sender
        {
            get { return sender; }
            set => Set(ref sender, nameof(Sender), value);
        }

        private Guid senderId;

        public Guid SenderId
        {
            get { return senderId; }
            set => Set(ref senderId, nameof(SenderId), value);
        }

        private DateTime publishedAt;

        public DateTime PublishedAt
        {
            get { return publishedAt; }
            set => Set(ref publishedAt, nameof(PublishedAt), value);
        }

        private MessageStatus status;

        public MessageStatus Status
        {
            get { return status; }
            set => Set(ref status, nameof(Status), value);
        }

        public ObservableCollection<AttachmentViewModel> Attachments { get; } = new ObservableCollection<AttachmentViewModel>();

        #endregion

        public MessageViewModel(IRoomHandler roomHandler, User user)
        {
            this.roomHandler = roomHandler;
            SenderId = user.Id;
            Sender = user.UserName;
        }

        private void MessageHandler_StatusChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Status));
        }

        public async Task Send()
        {

            if (messageHandler.CanBeSent())
            {
                MessageSent?.Invoke(this, EventArgs.Empty);
                await messageHandler.Send();
            }
        }

        public void AddAttachment(IFile file)
        {
            var vm = new AttachmentViewModel(file);
            vm.RemoveAttachment += AttachementVM_RemoveAttachment;
            Attachments.Add(vm);
        }

        private void AttachementVM_RemoveAttachment(object sender, EventArgs e)
        {
            var vm = (AttachmentViewModel)sender;

            messageHandler.Attachments.Remove(vm.File);
            Attachments.Remove(vm);
        }

        #region Commands

        private ICommand _sendCommand;

        public ICommand SendCommand => _sendCommand ?? (_sendCommand = new AsyncRelayCommand(Send));


        private ICommand _addAttachmentCommand;

        public ICommand AddAttachmentCommand => _addAttachmentCommand ?? (_addAttachmentCommand = new RelayCommand(AddAttachmentCommandHandler));

        private void AddAttachmentCommandHandler()
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;
                string fileName = Path.GetFileName(filePath);
                var stream = dialog.OpenFile();
                IFile file = messageHandler.AddAttachment(stream, stream.Length, fileName);
                file.UploadAsync();
                AddAttachment(file);
            }
        }

        #endregion
    }
}