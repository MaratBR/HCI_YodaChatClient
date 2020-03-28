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
        private List<SessionInfo> sessions;

        public AppConfigStore()
        {
        }

        public List<SessionInfo> GetSessions()
        {
            if (sessions == null)
            {
                sessions = LoadSessions();
            }

            return sessions;
        }

        private List<SessionInfo> LoadSessions()
        {
            if (sessions == null)
            {
                string value = Properties.Settings.Default.Sessions;
                value = Encryptions.Decrypt(value);
                List<SessionInfo> _sessions;
                if (value == null)
                {
                    _sessions = new List<SessionInfo>();
                }
                else
                {
                    try
                    {
                        _sessions = JsonConvert.DeserializeObject<List<SessionInfo>>(value);
                    }
                    catch
                    {
                        _sessions = new List<SessionInfo>();
                    }
                }

                sessions = _sessions;
            }
            return sessions;
        }

        public void SetSessions(List<SessionInfo> sessions)
        {
            var value = JsonConvert.SerializeObject(sessions);
            value = Encryptions.Encrypt(value);
            Properties.Settings.Default.Sessions = value;
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
    }
}
