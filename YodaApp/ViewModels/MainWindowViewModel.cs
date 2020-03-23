using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApp.Utils;
using YodaApp.Views;
using YodaApp.YODApi;
using YodaApp.YODApi.DataTypes;

namespace YodaApp.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly Api _api;

        public MainWindowViewModel(Api api)
        {
            _api = api;
        }

        #region Properties

        #endregion

        #region Commands

        private ICommand _logInCommand;

        public ICommand LogInCommand => _logInCommand ?? (_logInCommand = new RelayCommand(LogInCommandHandler));

        private void LogInCommandHandler()
        {
            var loginWindow = new LoginWindow();
            var loginWindowVM = new LoginViewModel();
            loginWindowVM.UserAuthenticated += LoginWindowVM_UserAuthenticated;
            loginWindow.DataContext = loginWindowVM;
            loginWindow.Show();
        }

        private void LoginWindowVM_UserAuthenticated(object sender, UserAuthenticatedEventArgs e)
        {

        }

        #endregion

    }
}
