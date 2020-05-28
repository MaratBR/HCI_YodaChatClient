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

        void SetCurrentSession(IApi api);

        void Logout();

        Task Init();

        IApiProvider GetApiProvider();

        event EventHandler SessionChanged;
        event EventHandler OnLogout;
        event EventHandler OnLogin;
    }
}
