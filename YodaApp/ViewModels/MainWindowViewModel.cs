﻿using Autofac;
using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApp.Controls;
using YodaApp.Services;
using YodaApp.Utils;
using YodaApp.ViewModels.Controls;

namespace YodaApp.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authentication;
        private readonly IAppUIService _windows;
        private readonly IComponentContext _componentContext;

        public MainWindowViewModel(IAuthenticationService authentication, IAppUIService windows, IComponentContext componentContext)
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

        #endregion Properties

        #region Commands

        private ICommand openProfileCommand;

        public ICommand OpenProfileCommand => openProfileCommand ?? (openProfileCommand = new AsyncRelayCommand(OpenProfile));

        private Task OpenProfile()
        {
            var vm = new ProfileViewModel(_authentication);
            var v = new Profile
            {
                DataContext = vm
            };
            return DialogHost.Show(v, "Default");
        }

        #endregion Commands

        private void Init()
        {
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
            if (Session != null)
                Session.Disconnect();

            Session = api == null ? null : CreateUserSessionViewModel(api);

            if (Session != null)
                Session.Update();
            else
                LogIn();
        }
    }
}