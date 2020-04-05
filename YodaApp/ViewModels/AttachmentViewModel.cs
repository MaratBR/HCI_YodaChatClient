using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApiClient.DataTypes;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{

    class AttachmentViewModel : ViewModelBase
    {
        public IFile File { get; }

        public string FileName => File.FileName;

        public string FileSize => File.Size >= 1024 * 1024 ? $"{Math.Round((decimal)File.Size / (1024 * 1024), 1)}M" :
                File.Size >= 1024 ? $"{Math.Round((decimal)File.Size / 1024, 1)}K" :
                $"{File.Size}B";

        public FileState State => File.State;

        public FileModel FileModel => File.FileModel;

        public string Status => State.GetDescription();

        public string Error => File.Error;

        public event EventHandler RemoveAttachment;

        private ICommand _removeCommand;

        public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new RelayCommand(RemoveCommandHandler));

        private ICommand _downloadCommand;

        public ICommand DownloadCommand => _downloadCommand ?? (_downloadCommand = new AsyncRelayCommand(DownloadCommandHandler));

        private async Task DownloadCommandHandler()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = FileModel?.FileName;
            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;
                var fileStream = System.IO.File.OpenWrite(filePath);
                await File.DownloadToAsync(fileStream);
            }
        }

        private void RemoveCommandHandler()
        {
            RemoveAttachment?.Invoke(this, EventArgs.Empty);
        }

        public AttachmentViewModel(IFile file)
        {
            this.File = file;
            file.StateChanged += File_StateChanged;
        }

        ~AttachmentViewModel()
        {
            File.StateChanged -= File_StateChanged;
        }

        private void File_StateChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(FileModel));
            OnPropertyChanged(nameof(Error));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(FileName));
        }
    }
}
