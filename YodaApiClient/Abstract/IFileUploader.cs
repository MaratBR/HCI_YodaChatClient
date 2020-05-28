using System;
using System.IO;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient
{
    public interface IFileUploader
    {
        Task<FileModel> UploadFileAsync(FileStream stream, string fileName);

        Task<FileModel> GetFileModelAsync(Guid id);
    }
}