using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApiClient
{
    public class SessionInfo
    {
        public string Token { get; set; }

        public Guid RefreshToken { get; set; }

        public Guid UserId { get; set; }

        public Guid SessionId { get; set; } = Guid.NewGuid();

        public DateTime ExpiresAt { get; set; }
    }
}
