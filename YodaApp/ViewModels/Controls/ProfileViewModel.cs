using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApiClient.DataTypes;
using YodaApp.Services;
using YodaApp.Utils;

namespace YodaApp.ViewModels.Controls
{
    class ProfileViewModel : ViewModelBase
    {
        private readonly IAuthenticationService authentication;

        public ProfileViewModel(IAuthenticationService authentication)
        {
            this.authentication = authentication;
        }

        private ICommand initCommand;

        public ICommand InitCommand => initCommand ?? (initCommand = new AsyncRelayCommand(Init));

        private async Task Init()
        {
            User = await authentication.GetCurrentSession().GetUserAsync();
        }

        public User User { get; set; }

        private ICommand logoutCommand;

        public ICommand LogoutCommand => logoutCommand ?? (logoutCommand = new RelayCommand(Logout));

        private void Logout()
        {
            authentication.Logout();
        }
    }
}
