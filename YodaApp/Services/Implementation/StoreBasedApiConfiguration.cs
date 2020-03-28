using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient;
using YodaApp.Persistence;

namespace YodaApp.Services.Implementation
{
    class StoreBasedApiConfiguration : ApiConfiguration
    {

        public StoreBasedApiConfiguration(IStore store)
        {
            UseHTTPS = store.GetBool("api.https") ?? UseHTTPS;
            Domain = store.Get("api.domain") ?? Domain;
            BasePath = store.Get("api.basePath") ?? BasePath;
        }
    }
}
