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
        private readonly IFile file;

        public string FileName => file.FileName;

        public string FileSize => file.Size >= 1024 * 1024 ? $"{Math.Round((decimal)file.Size / (1024 * 1024), 1)}M" :
                file.Size >= 1024 ? $"{Math.Round((decimal)file.Size / 1024, 1)}K" :
                $"{file.Size}B";

        public FileState State => file.State;

        public FileModel FileModel => file.FileModel;

        public string Status => State.GetDescription();

        public string Error => file.Error;

        public event EventHandler RemoveAttachment;

        private ICommand _removeCommand;

        public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new RelayCommand(RemoveCommandHandler));

        private void RemoveCommandHandler()
        {
            RemoveAttachment?.Invoke(this, EventArgs.Empty);
        }

        public AttachmentViewModel(IFile file)
        {
            this.file = file;
            file.StateChanged += File_StateChanged;
        }

        ~AttachmentViewModel()
        {
            file.StateChanged -= File_StateChanged;
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
