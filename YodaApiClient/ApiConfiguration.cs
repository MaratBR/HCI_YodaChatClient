using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient
{
    public class ApiConfiguration
    {
        public string Domain { get; set; } = "localhost:1620";

        public bool UseHTTPS { get; set; } = false;

        public string BasePath { get; set; } = "api";

        public string FullURI => new StringBuilder()
            .Append(UseHTTPS ? "https" : "http")
            .Append("://")
            .Append(Domain)
            .Append("/")
            .Append(BasePath)
            .ToString();

        public string AppendPathToMainUrl(string path) => FullURI + (path.StartsWith("/") ? path : ("/" + path));
    }
}
