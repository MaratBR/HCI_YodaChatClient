using System;
using System.Threading.Tasks;
using YodaApiClient;
using YodaApiClient.Abstract;

namespace YodaApp.Services
{
    internal interface IAuthenticationService
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