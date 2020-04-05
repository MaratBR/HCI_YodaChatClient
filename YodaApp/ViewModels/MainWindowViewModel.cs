using Autofac;
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
        private readonly IComponentContext _componentContext;

        public MainWindowViewModel(IAuthenticationService authentication, IWindowService windows, IComponentContext componentContext)
        {
            _windows = windows;
            _authentication = authentication;
            _authentication.SessionChanged += Authentication_SessionChanged;
            _componentContext = componentContext;
            Init();
        }

        private void Authentication_SessionChanged(object sender, EventArgs e)
        {
            SetCurrentSession(_authentication.GetCurrentSession());
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

        #endregion

        #region Commands

        private ICommand _logInCommand;

        public ICommand LogInCommand => _logInCommand ?? (_logInCommand = new RelayCommand(LogIn));

        #endregion

        private async void Init()
        {
            var sessions = _authentication.GetSessions();

            UserSessions = new ObservableCollection<UserSessionViewModel>(
                sessions.Select(CreateUserSessionViewModel)
                );

            SetCurrentSession(_authentication.GetCurrentSession());
        }

        private UserSessionViewModel CreateUserSessionViewModel(IApi api)
        {
            return _componentContext.Resolve<UserSessionViewModel>(new TypedParameter(typeof(IApi), api));
        }

        private void LogIn()
        {
            _windows.ShowLogInWindow();
            _windows.HideMainWindow();
        }

        private void SetCurrentSession(IApi api)
        {
            UserSessionViewModel session = userSessions.SingleOrDefault(s => s.Api == api);
            if (session == null)
            {
                session = CreateUserSessionViewModel(api);
                UserSessions.Add(session);
            }
            SetCurrentSession(session);
        }

        private void SetCurrentSession(UserSessionViewModel userSession)
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
