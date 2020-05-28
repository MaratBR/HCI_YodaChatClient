using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient;
using YodaApiClient.Abstract;
using YodaApp.Persistence;

namespace YodaApp.Services.Implementation
{
    class AuthenticationService : IAuthenticationService
    {
        private readonly IStore _store;
        private readonly IApiProvider _apiProvider;
        private IApi currentSession;

        public event EventHandler SessionChanged;
        public event EventHandler OnLogout;
        public event EventHandler OnLogin;

        public AuthenticationService(IStore store, IApiProvider apiProvider)
        {
            _store = store;
            _apiProvider = apiProvider;
        }
        

        public IApi GetCurrentSession() => currentSession;

        public bool HasAuthenticatedSession() => currentSession != null;

        public async Task Init()
        {
            var sessionInfo = _store.GetSession();
            IApi api = null;
            if (sessionInfo != null)
            {
                try
                {
                    api = await _apiProvider.CreateApi(sessionInfo);
                }
                catch
                {

                }
            }

            SetCurrentSession(api);
        }

        public void SetCurrentSession(IApi api)
        {;
            currentSession = api;
            if (api != null)
                _store.SetSession(api.GetSessionInfo());

            SessionChanged?.Invoke(this, EventArgs.Empty);
            OnLogin?.Invoke(this, EventArgs.Empty);
            SessionChanged?.Invoke(this, EventArgs.Empty);
        }


        public IApiProvider GetApiProvider() => _apiProvider;

        public void Logout()
        {
            _store.SetSession(null);
            currentSession = null;
            OnLogout?.Invoke(this, EventArgs.Empty);
            SessionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
