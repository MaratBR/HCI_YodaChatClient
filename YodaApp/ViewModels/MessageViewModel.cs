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
        Draft,
        Sending,
        Sent,
        Error,
        Received
    }

    class MessageViewModel : ViewModelBase
    {
        private readonly IRoomHandler roomHandler;

        public event EventHandler MessageSubmitted;

        #region Properties

        private long? id;

        public long? Id
        {
            get { return id; }
            set => Set(ref id, nameof(Id), value);
        }


        private string error;

        public string Error
        {
            get { return error; }
            set => Set(ref error, nameof(Error), value);
        }


        private string text;

        public string Text
        {
            get { return text; }
            set
            {
                Set(ref text, nameof(Text), value);
                OnPropertyChanged(nameof(HasMoreButton));
            }
        }

        private bool displayAll;

        public bool DisplayAll
        {
            get { return displayAll; }
            set
            {
                Set(ref displayAll, nameof(DisplayAll), value);
                OnPropertyChanged(nameof(DisplayedText));
            }
        }


        public string DisplayedText => DisplayAll || !HasMoreButton ? Text : (Text.Substring(0, 300) + "...");

        public bool HasMoreButton => Text != null && Text.Length > 300;

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
            set
            {
                Set(ref status, nameof(Status), value);
                OnPropertyChanged(nameof(IsPersistent));
                OnPropertyChanged(nameof(IsSending));
            }
        }

        public ObservableCollection<AttachmentViewModel> Attachments { get; } = new ObservableCollection<AttachmentViewModel>();

        public bool IsSelf { get; }

        public bool IsPersistent => Status == MessageStatus.Received || Status == MessageStatus.Sent;

        public bool IsSending => Status == MessageStatus.Sending;

        #endregion

        public MessageViewModel(IRoomHandler roomHandler)
        {
            this.roomHandler = roomHandler;

            SenderId = roomHandler.Client.User.Id;
            Sender = roomHandler.Client.User.UserName;
            IsSelf = true;
            Status = MessageStatus.Draft;
        }

        public MessageViewModel(IRoomHandler roomHandler, ChatMessageDto messageDto)
        {
            this.roomHandler = roomHandler;

            SenderId = messageDto.Sender.Id;
            Sender = messageDto.Sender.UserName;
            IsSelf = messageDto.Sender.Id == roomHandler.Client.User.Id;
            Status = MessageStatus.Received;
            Text = messageDto.Text;

            foreach (var attachment in messageDto.Attachments)
            {
                AddAttachment(attachment);
            }
        }

        public async Task Send()
        {
            if (!IsPersistent && IsSelf && !IsSending)
            {
                MessageSubmitted?.Invoke(this, EventArgs.Empty);
                Status = MessageStatus.Sending;
                var request = new ChatMessageRequestDto
                {
                    RoomId = roomHandler.Id,
                    Text = Text,
                    Attachments = Attachments.Select(a => a.Id).ToList()
                };
                var result = await roomHandler.PutToQueue(request);

                if (result.Sent)
                {
                    Status = MessageStatus.Sent;
                    Id = result.Id;
                }
                else
                {
                    Status = MessageStatus.Error;
                    Error = result.Error;
                }
            }
        }

        private void AddAttachment(ChatAttachmentDto attachmentDto)
        {
            AddAttachment(new AttachmentViewModel(roomHandler.Client.Api, attachmentDto));
        }

        private void AddAttachment(FileInfo fileInfo)
        {
            var vm = new AttachmentViewModel(roomHandler.Client.Api, fileInfo);
            var task = vm.EnsureServerSidePersistence();
            // TODO do smth with tasks
            AddAttachment(vm);
        }

        private void AddAttachment(AttachmentViewModel vm)
        {
            vm.RemoveAttachment += AttachementVM_RemoveAttachment;
            Attachments.Add(vm);
        }

        private void AttachementVM_RemoveAttachment(object sender, EventArgs e)
        {
            var vm = (AttachmentViewModel)sender;
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
                var fileInfo = new FileInfo(filePath);
                AddAttachment(fileInfo);
            }
        }

        private ICommand _toggleTextCommand;

        public ICommand ToggleTextCommand => _toggleTextCommand ?? (_toggleTextCommand = new RelayCommand(ToggleTextCommandHandler, () => HasMoreButton));

        private void ToggleTextCommandHandler()
        {
            DisplayAll = !DisplayAll;
        }

        #endregion
    }
}