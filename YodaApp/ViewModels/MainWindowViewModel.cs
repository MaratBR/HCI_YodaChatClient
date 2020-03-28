using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApp.Persistence;
using YodaApp.Services;
using YodaApp.Utils;
using YodaApp.Views;

namespace YodaApp.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authentication;
        private readonly IWindowService _windows;

        public MainWindowViewModel(IAuthenticationService authentication, IWindowService windows)
        {
            _authentication = authentication;
            Init();
        }

        #region Properties

        private UserSessionViewModel session;

        public UserSessionViewModel Session
        {
            get => session;
            set => Set(ref session, nameof(Session), value);
        }

        private ObservableCollection<UserSessionViewModel> userSessions = new ObservableCollection<UserSessionViewModel>();

        public ObservableCollection<UserSessionViewModel> UserSessions
        {
            get => userSessions;
            set => Set(ref userSessions, nameof(UserSessions), value);
        }

        private bool isWindowHidden = true;

        public bool IsWindowHidden
        {
            get => isWindowHidden;
            set => Set(ref isWindowHidden, nameof(IsWindowHidden), value);
        }

        #endregion

        #region Commands

        private ICommand _logInCommand;

        public ICommand LogInCommand => _logInCommand ?? (_logInCommand = new RelayCommand(LogIn));

        #endregion

        private async void Init()
        {
            var sessions = _authentication.GetSessions();

            UserSessions = new ObservableCollection<UserSessionViewModel>(
                sessions.Select((api) => new UserSessionViewModel(api))
                );

            await SetApi(_authentication.GetCurrentSession());
        }

        private async Task SetApi(IApi api)
        {
            var session = UserSessions.Where(s => s.Api == api).SingleOrDefault();

            await SetCurrentSession(session);
        }

        private void LogIn()
        {
            _windows.ShowLogInWindow();
            _windows.HideMainWindow();
        }

        private async void LoginWindowVM_UserAuthenticated(object sender, UserAuthenticatedEventArgs e)
        {
            IsWindowHidden = false;
            await AddSession(e.Api);
        }

        private async Task AddSession(IApi api)
        {
            var sessionVM = new UserSessionViewModel(api);
            UserSessions.Add(sessionVM);
            await SetCurrentSession(sessionVM);
        }

        private async Task SetCurrentSession(UserSessionViewModel userSession)
        {
            if (Session != null)
                Session.Disconnect();

            Session = userSession;

            if (Session != null)
                Session.Update();
            else
                LogIn();
        }
    }
}
