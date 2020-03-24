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
using YodaApp.Persistence;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
	class UserAuthenticatedEventArgs : EventArgs
	{
		public User User { get; set; }
	}

    class LoginViewModel : ViewModelBase
    {
		private readonly IApiProvider apiProvider;

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


		public event EventHandler<UserAuthenticatedEventArgs> UserAuthenticated;

		#region Log in

		private ICommand loginCommand;

		public ICommand LoginCommand => loginCommand ?? (loginCommand = new AsyncRelayCommand<PasswordBox>(LoginCommandHandler));

		public async Task LoginCommandHandler(PasswordBox passwordBox)
		{
			Loading = true;
			var pwd = passwordBox.Password;

			IApi api;

			try
			{
				api = await apiProvider.CreateApi(new AuthenticationRequest { Login = Login, Password = pwd });
			}
			catch(ApiException exc)
			{
				HasError = true;
				Error = exc.Message;
				return;
			}

			var user = await api.GetUserAsync();
			NotifyOnUserAuthenticated(user);

			Loading = false;
		}

		#endregion


		private void NotifyOnUserAuthenticated(User user)
		{
			UserAuthenticated?.Invoke(this, new UserAuthenticatedEventArgs { User = user });
		}
	}
}
