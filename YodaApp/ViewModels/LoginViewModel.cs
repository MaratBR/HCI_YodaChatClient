using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YodaApiClient;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.Requests;
using YodaApp.Persistence;
using YodaApp.Services;
using YodaApp.Utils;
using YodaApp.Views;

namespace YodaApp.ViewModels
{
    class UserAuthenticatedEventArgs : EventArgs
	{
		public IApi Api { get; set; }
	}

    class LoginViewModel : ViewModelBase
	{
		private readonly IAuthenticationService _authentication;
		private readonly IAppUIService _windows;

        #region Properties

        private string login;

		public string Login
		{
			get { return login; }
			set { Set(ref login, nameof(Login), value); }
		}

		private bool loading;

		public bool Loading
		{
			get { return loading; }
			set { Set(ref loading, nameof(Loading), value); }
		}

		private string error;

		public string Error
		{
			get { return error; }
			set { Set(ref error, nameof(Error), value); }
		}

		private bool hasError;

		public bool HasError
		{
			get { return hasError; }
			set { Set(ref hasError, nameof(HasError), value); }
		}

		#endregion


		public LoginViewModel(IAuthenticationService authentication, IAppUIService windows)
		{
			_authentication = authentication;
			_windows = windows;
		}

		#region Commands

		private ICommand loginCommand;

		public ICommand LoginCommand => loginCommand ?? (loginCommand = new AsyncRelayCommand<PasswordBox>(LoginCommandHandler));

		public async Task LoginCommandHandler(PasswordBox passwordBox)
		{
			var pwd = passwordBox.Password;

			await PerformLogin(Login, pwd);
		}

		private ICommand signUpCommand;

		public ICommand SignUpCommand => signUpCommand ?? (signUpCommand = new RelayCommand(SignUp));

		private void SignUp()
		{
			_windows.ShowSignUpWindow();
			_windows.CloseLogInWindow();
		}

		#endregion

		public async Task PerformLogin(string login, string password)
		{
			Loading = true;

			IApi api;

			try
			{
				api = await _authentication.GetApiProvider().CreateApi(new AuthenticationRequest { Login = login, Password = password });
			}
			catch (ApiException exc)
			{
				HasError = true;
				Error = exc.Message;
				Loading = false;
				return;
			}

			Loading = false;


			_authentication.SetCurrentSession(api);
			_windows.ShowMainWindow();
			_windows.CloseLogInWindow();
		}
	}
}
