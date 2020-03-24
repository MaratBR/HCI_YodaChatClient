using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApp.Utils;
using YodaApp.Views;

namespace YodaApp.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
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

        private void LoginWindowVM_UserAuthenticated(object sender, UserAuthenticatedEventArgs e)
        {

        }

        #endregion

        private void LogIn()
        {
            var loginWindow = new LoginWindow();
            var loginWindowVM = new LoginViewModel();
            loginWindowVM.UserAuthenticated += LoginWindowVM_UserAuthenticated;
            loginWindow.DataContext = loginWindowVM;
            loginWindow.Show();
        }

        private void AddSession(IApi api)
        {

        }
    }
}
