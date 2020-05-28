using System;

namespace YodaApiClient
{
    public class SessionInfo
    {
        public string Token { get; set; }

        public Guid RefreshToken { get; set; }

        public int UserId { get; set; }

        public Guid SessionId { get; set; } = Guid.NewGuid();

        public DateTime ExpiresAt { get; set; }
    }
}