using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient;

namespace YodaApp.Persistence
{
    class AppConfigStore : IStore
    {
        public AppConfigStore()
        {
        }

        public void SetSessions(List<SessionInfo> sessions)
        {
            var value = JsonConvert.SerializeObject(sessions);
            value = Encryptions.Encrypt(value);
            Properties.Settings.Default.SessionEnc = value;
            Properties.Settings.Default.Save();
        }

        public ApiConfiguration GetConfiguration()
        {
            if (Properties.Settings.Default.ApiConfiguration == null)
            {
                SetConfiguration(new ApiConfiguration());
            }
            return Properties.Settings.Default.ApiConfiguration;
        }

        public void SetConfiguration(ApiConfiguration configuration)
        {
            Properties.Settings.Default.ApiConfiguration = configuration;
            Properties.Settings.Default.Save();
        }

        public void SetSession(SessionInfo info)
        {
            var data = JsonConvert.SerializeObject(info);
            Properties.Settings.Default.SessionEnc = Encryptions.Encrypt(data);
            Properties.Settings.Default.Save();
        }

        public SessionInfo GetSession()
        {
            Properties.Settings.Default.Reload();
            
            try
            {
                var value = Encryptions.Decrypt(Properties.Settings.Default.SessionEnc);
                var info = JsonConvert.DeserializeObject<SessionInfo>(value);
                return info;
            }
            catch
            {
                return null;
            }
        }
    }
}
