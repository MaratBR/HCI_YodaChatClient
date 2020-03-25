using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApp.ViewModels
{

    class AttachmentViewModel : ViewModelBase
    {
        public string FileName { get; private set; }

        public string FileSize { get; private set; }

        private bool uploaded;

        public bool Uploaded
        {
            get => uploaded;
            set
            {
                Set(ref uploaded, nameof(Uploaded), value);
                OnPropertyChanged(nameof(Status));
            }
        }

        private FileModel fileModel;

        public FileModel FileModel
        {
            get { return fileModel; }
            set => Set(ref fileModel, nameof(FileModel), value);
        }

        public string Status => Uploaded ? "Done" : "Uploading...";


        public AttachmentViewModel(string fileName, long size)
        {
            FileName = fileName;
            FileSize = size >= 1024 * 1024 ? $"{Math.Round((decimal)size / (1024 * 1024), 1)}M" :
                size >= 1024 ? $"{Math.Round((decimal)size / 1024, 1)}K" :
                $"{size}B";
        }
    }
}
