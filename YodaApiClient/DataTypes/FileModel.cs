using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes
{
    public class FileModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Sha256 { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }
        public long Size { get; set; }
    }
}
