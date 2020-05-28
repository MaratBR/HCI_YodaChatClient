using System.Collections.Generic;

namespace YodaApiClient.DataTypes
{
    public class YODAError
    {
        public string Description { get; set; }
        public string Details { get; set; }
        public object Extra { get; set; }
        public ICollection<YODAError> Errors { get; set; }
    }
}