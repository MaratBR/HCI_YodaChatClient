using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient;
using YodaApiClient.Abstract;

namespace YodaApp.Services
{
    interface IAuthenticationService
    {
        bool HasAuthenticatedSession();

        IApi GetCurrentSession();

        void AddSession(IApi api);

        void RemoveSession(IApi api);

        void SetCurrentSession(IApi api);

        List<IApi> GetSessions();

        Task Init();

        IApiProvider GetApiProvider();

        event EventHandler SessionChanged;
    }
}
