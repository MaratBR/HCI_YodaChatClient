using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient.Implementation
{
    class FileImpl : IFile
    {
        private readonly Guid id;
        private readonly IApi api;

        private Stream stream;
        private long fileSize;
        private string fileName;

        public event EventHandler StateChanged;

        public FileImpl(Guid id, IApi api)
        {
            this.id = id;
            this.api = api;
        }

        public FileImpl(Stream fstream, string fileName, long fileSize, IApi api)
        {
            stream = fstream;
            this.api = api;
            this.fileSize = fileSize;
            this.fileName = fileName;
        }

        public async Task Upload()
        {
            if (stream == null)
                return;

            State = FileState.Uploading;
            FileModel fm;
            try
            {
                fm = await api.UploadFile(stream, fileName);
            }
            catch
            {
                State = FileState.NotUploaded;
                // TODO error
                return;
            }
            FileModel = fm;
            State = FileState.Uploaded;
            stream.Dispose();
            stream = null;
        }

        public async Task LoadModel()
        {
            State = FileState.Loading;
            try
            {
                FileModel = await api.LoadFileModel(Id);
            }
            catch
            {
                State = FileState.NotLoaded;
                // TODO error
                return;
            }
            State = FileState.Loaded;
        }

        public Guid Id => FileModel.Id;

        private FileState state;

        public FileState State
        {
            get => state;
            set
            {
                StateChanged?.Invoke(this, EventArgs.Empty);
                state = value;
            }
        }

        public FileModel FileModel { get; private set; }

        public string FileName => FileModel == null ? fileName : FileModel.FileName;

        public long Size => FileModel == null ? fileSize : FileModel.Size;
    }
}
