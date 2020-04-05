using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient.DataTypes;

namespace YodaApiClient
{
    public interface IFileUploader
    {
        Task<FileModel> UploadFileAsync(Stream stream, string fileName);

        Task<FileModel> GetFileModelAsync(Guid id);
    }
}
