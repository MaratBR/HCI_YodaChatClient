using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace YodaApp.YODApi
{
    class Api
    {
        private readonly string apiDomain;
        private readonly ApiConfiguration configuration;
        private readonly HttpClient client;
        private readonly ClientWebSocket wsClient;

        public Api(ApiConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            wsClient = new ClientWebSocket();
        }

        public async Task<AuthenticationResult> Authenticate(string username, string password)
        {

        }
    }
}
