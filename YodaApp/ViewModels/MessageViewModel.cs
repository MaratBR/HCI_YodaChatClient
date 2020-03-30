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
        private readonly IMessageHandler messageHandler;

        public event EventHandler MessageSent;

        public MessageViewModel(IMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
            Attachments = new ObservableCollection<AttachmentViewModel>(
                messageHandler.Attachments.Select(file => new AttachmentViewModel(file))
                );
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
            Attachments.Add(new AttachmentViewModel(file));
        }

        #region Properties


        public string Text
        {
            get => messageHandler.Text;
            set
            {
                if (messageHandler.Text == value)
                    return;
                messageHandler.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public ObservableCollection<AttachmentViewModel> Attachments { get; }


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


        private ICommand _addAttachmentCommand;

        public ICommand AddAttachmentCommand => _addAttachmentCommand ?? (_addAttachmentCommand = new AsyncRelayCommand(AddAttachmentCommandHandler));

        private async Task AddAttachmentCommandHandler()
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;
                string fileName = Path.GetFileName(filePath);
                var stream = dialog.OpenFile();
                IFile file = messageHandler.AddAttachment(stream, stream.Length, fileName);
                await file.Upload();
                AddAttachment(file);
            }
        }

        #endregion
    }
}