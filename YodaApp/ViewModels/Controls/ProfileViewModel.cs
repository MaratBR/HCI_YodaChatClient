﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient.DataTypes;
using YodaApp.Controls;
using YodaApp.Services;
using YodaApp.Utils;

namespace YodaApp.ViewModels.Controls
{
    internal class ProfileViewModel : ViewModelBase
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
            try
            {
                User = await authentication.GetCurrentSession().GetUserAsync();
                OnPropertyChanged(nameof(User));
            }
            catch(Exception e)
            {
                await ErrorView.Show(e);
            }
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