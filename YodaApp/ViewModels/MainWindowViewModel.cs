using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApp.Persistence;
using YodaApp.Utils;
using YodaApp.Views;

namespace YodaApp.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly IStore store = new AppConfigStore();
        private readonly IApiProvider apiProvider;

        public MainWindowViewModel()
        {
            apiProvider = new ApiProvider(new ApiConfiguration());
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

        public LoginViewModel LoginVM
        {
            get
            {
                var vm = new LoginViewModel(apiProvider);
                vm.UserAuthenticated += LoginWindowVM_UserAuthenticated;
                return vm;
            }
        }

        private bool isWindowHidden = false;

        public bool IsWindowHidden
        {
            get => isWindowHidden;
            set => Set(ref isWindowHidden, nameof(IsWindowHidden), value);
        }


        #endregion

        #region Commands

        private ICommand _logInCommand;

        public ICommand LogInCommand => _logInCommand ?? (_logInCommand = new RelayCommand(LogIn));


        private ICommand _loadedCommand;

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new AsyncRelayCommand(Init));


        #endregion

        private async Task Init()
        {
            var sessions = store.GetSessions();

            UserSessions = new ObservableCollection<UserSessionViewModel>(
                sessions.Select(
                    async (s) =>
                    {
                        try
                        {
                            var api = await apiProvider.CreateApi(s);

                            return new UserSessionViewModel(api);
                        }
                        catch(ApiException)
                        {
                            return null;
                        }
                    })
                    .Select(t => t.GetAwaiter().GetResult())
                    .Where(r => r != null)
                );

            if (UserSessions.Count != 0)
                Session = UserSessions.First();

            if (Session == null)
            {
                LogIn();
            }
        }

        private void LogIn()
        {
            IsWindowHidden = true;
            var wnd = new LoginWindow();
            var vm = (LoginViewModel)wnd.DataContext;
            vm.UserAuthenticated += (object sender, UserAuthenticatedEventArgs e) =>
            {
                wnd.Close();
            };
            wnd.ShowDialog();
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
                await Session.Update();
        }
    }
}
