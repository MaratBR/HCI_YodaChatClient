using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient;
using YodaApp.Persistence;

namespace YodaApp.Services.Implementation
{
    class AuthenticationService : IAuthenticationService
    {
        private readonly IStore _store;
        private readonly IApiProvider _apiProvider;
        private readonly IDictionary<Guid, IApi> apis = new Dictionary<Guid, IApi>();
        private IApi currentSession;

        public AuthenticationService(IStore store, IApiProvider apiProvider)
        {
            _store = store;
            _apiProvider = apiProvider;
        }

        public void AddSession(IApi api)
        {
            var sessions = _store.GetSessions();
            sessions.Insert(0, api.GetSessionInfo());
            _store.SetSessions(sessions);
            apis[api.GetGuid()] = api;
        }

        public IApi GetCurrentSession() => currentSession;

        public List<IApi> GetSessions() => apis.Values.ToList();

        public bool HasAuthenticatedSession() => currentSession != null;

        /// <summary>
        /// Достаёт из IStore информацию о сессиях, создаёт для каждой IApi
        /// и сохраняет в список
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
            var sessions = _store.GetSessions();

            List<IApi> apis;

            {
                IApi[] _apis = await Task.WhenAll(
                    sessions.Select(CreateApiOrNull)
                    );

                apis = _apis.Where(api => api != null).ToList();
            }

            apis.ForEach(api =>
            {
                this.apis[api.GetGuid()] = api;
            });
        }

        private async Task<IApi> CreateApiOrNull(SessionInfo session)
        {
            try
            {
                return await _apiProvider.CreateApi(session);
            }
            catch (ApiException)
            {
                // TODO ???
                return null;
            }
        }

        public void RemoveSession(IApi api)
        {
            if (apis.ContainsKey(api.GetGuid()))
                apis.Remove(api.GetGuid());
            if (api.GetGuid() == currentSession.GetGuid())
                FindSession();
        }

        public void SetCurrentSession(IApi api)
        {
            if (api != null)
                if (!apis.ContainsKey(api.GetGuid()))
                    AddSession(api);
            currentSession = api;
        }

        public void FindSession()
        {
            if (apis.Count == 0)
            {
                SetCurrentSession(null);
            }
            else
            {
                SetCurrentSession(apis.Values.First());
            }

        }

        public IApiProvider GetApiProvider() => _apiProvider;
    }
}
