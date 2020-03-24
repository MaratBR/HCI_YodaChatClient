using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient.DataTypes
{
    class YODAError
    {
        public string Description { get; set; }
        public string Details { get; set; }
        public object Extra { get; set; }
        public ICollection<YODAError> Errors { get; set; }
    }
}
