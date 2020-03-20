using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApp.YODApi
{
    class ApiConfiguration
    {
        public string Domain { get; set; } = "localhost:1620";

        public bool UseHTTPS { get; set; } = false;

        public string BaseUrl { get; set; } = "/api";
    }
}
