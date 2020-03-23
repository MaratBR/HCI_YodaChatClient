using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApp.Persistence
{
    class AppConfigStore : IStore
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

        public void Set(string key, string value)
        {
            ConfigurationManager.AppSettings.Set(key, value);
        }
    }
}
