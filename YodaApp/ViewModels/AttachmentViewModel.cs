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
using YodaApiClient.DataTypes.DTO;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
    enum FileState : byte
    {
        Uploading,
        Loading,
        Error,
        OK
    }

    class AttachmentViewModel : ViewModelBase
    {
        private readonly IApi api;
        private FileModel model;


        public string FullPath { get; set; }

        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set => Set(ref fileName, nameof(FileName), value);
        }


        private Guid id;

        public Guid Id
        {
            get { return id; }
            set => Set(ref id, nameof(Id), value);
        }

        public int Size { get; set; }

        public string FileSize => Size >= 1024 * 1024 ? $"{Math.Round((decimal)Size / (1024 * 1024), 1)}M" :
                Size >= 1024 ? $"{Math.Round((decimal)Size / 1024, 1)}K" :
                $"{Size}B";

        private bool hasSpinner;

        public bool HasSpinner
        {
            get { return hasSpinner; }
            set => Set(ref hasSpinner, nameof(HasSpinner), value);
        }

        private bool hasError;

        public bool HasError
        {
            get { return hasError; }
            set => Set(ref hasError, nameof(HasError), value);
        }


        public event EventHandler RemoveAttachment;

        private ICommand _removeCommand;

        public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new RelayCommand(RemoveCommandHandler));

        private ICommand _downloadCommand;

        public ICommand DownloadCommand => _downloadCommand ?? (_downloadCommand = new AsyncRelayCommand(DownloadCommandHandler));

        private async Task DownloadCommandHandler()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = FileName;
            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;
                var fileStream = System.IO.File.OpenWrite(filePath);
                HasSpinner = true;

                try
                {
                    await api.DownloadFileAsync(Id, fileStream);
                }
                catch
                {
                    HasError = true;
                }
                finally
                {
                    HasSpinner = false;
                }

            }
        }

        private void RemoveCommandHandler()
        {
            RemoveAttachment?.Invoke(this, EventArgs.Empty);
        }

        public AttachmentViewModel(IApi api, ChatAttachmentDto file)
        {
            this.api = api;
            FileName = file.Name;
            Size = file.Size;
            Id = file.Id;
        }

        public AttachmentViewModel(IApi api, FileInfo fileInfo)
        {
            this.api = api;
            FullPath = fileInfo.FullName;
            FileName = fileInfo.Name;
            Size = (int)fileInfo.Length;
        }

        public async Task EnsureServerSidePersistence()
        {
            if (!Id.Equals(Guid.Empty))
            {
                if (model == null)
                    await LoadModel();

                return;
            }

            if (FullPath == null)
            {
                throw new InvalidOperationException("Attachment has empty GUID, hence considered to be local, but path to the local file is missing");
            }

            var stream = new FileStream(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            HasSpinner = true;
            try
            {
                FileModel fm = await api.UploadFileAsync(stream, FileName);
                Id = fm.Id;
                FileName = fm.FileName;
            }
            catch
            {
                HasError = true;
            }
            finally
            {
                stream.Dispose();
                HasSpinner = false;
            }
        }

        private async Task LoadModel()
        {
            model = await api.GetFileModelAsync(Id);
        }
    }
}
