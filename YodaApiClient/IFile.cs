using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient
{
    public enum FileState
    {
        [Description("Not uploaded yet")]
        NotUploaded,

        [Description("Not loaded yet")]
        NotLoaded,

        [Description("Uploading...")]
        Uploading,

        [Description("Loading...")]
        Loading,

        Loaded,
        Uploaded
    }

    public interface IFile
    {
        Guid Id { get; }

        FileState State { get; }

        FileModel FileModel { get; }

        string FileName { get; }

        long Size { get; }

        Task Upload();

        Task LoadModel();

        event EventHandler StateChanged;
    }
}
